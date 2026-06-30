using CoffeeChainManagement.Application.DTOs.Auth;
using CoffeeChainManagement.Application.DTOs.Audit;
using CoffeeChainManagement.Application.DTOs.Common;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Infrastructure.Auth;
using CoffeeChainManagement.Infrastructure.Persistence;
using CoffeeChainManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Tests.TestDoubles;

namespace CoffeeChainManagement.Tests.Infrastructure;

public sealed class AuthServiceTests
{
    [Fact]
    public async Task Login_returns_access_and_refresh_tokens()
    {
        await using var db = CreateDb();
        var user = CreateUser();
        db.Employees.Add(user);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var response = await service.LoginAsync(new LoginRequestDto("admin", "Password@123"));

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(response.RefreshToken));
        Assert.Equal("admin", response.User.Username);
        Assert.Single(db.RefreshTokenSessions);
    }

    [Fact]
    public async Task Refresh_rotates_refresh_token()
    {
        await using var db = CreateDb();
        var user = CreateUser();
        db.Employees.Add(user);
        await db.SaveChangesAsync();

        var service = CreateService(db);
        var login = await service.LoginAsync(new LoginRequestDto("admin", "Password@123"));
        var refreshed = await service.RefreshAsync(new RefreshRequestDto(login!.RefreshToken));

        Assert.NotNull(refreshed);
        Assert.NotEqual(login.RefreshToken, refreshed!.RefreshToken);
        Assert.Equal(2, await db.RefreshTokenSessions.CountAsync());
    }

    private static PostgresAuthService CreateService(CoffeeChainDbContext db)
        => new(
            db,
            new PasswordHasher<Employee>(),
            new NoopAuditLogService(),
            new TestCurrentUserContext(Guid.NewGuid(), "admin", null, UserRole.Administrator),
            Options.Create(new JwtOptions
            {
                Issuer = "CoffeeChainManagement.Api",
                Audience = "CoffeeChainManagement.Frontend",
                SigningKey = "0123456789abcdef0123456789abcdef",
                ExpiryMinutes = 60,
                RefreshExpiryDays = 7
            }));

    private static CoffeeChainDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<CoffeeChainDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CoffeeChainDbContext(options);
    }

    private static Employee CreateUser()
    {
        var user = new Employee
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            FullName = "System Admin",
            Email = "admin@coffeechain.local",
            PasswordHash = string.Empty,
            Role = CoffeeChainManagement.Domain.Enums.UserRole.Administrator,
            BranchId = null,
            IsActive = true
        };

        user.PasswordHash = new PasswordHasher<Employee>().HashPassword(user, "Password@123");
        return user;
    }

    private sealed class NoopAuditLogService : IAuditLogService
    {
        public Task<PagedResultDto<AuditLogDto>> GetPagedAsync(AuditLogQueryDto query, CancellationToken cancellationToken = default)
            => Task.FromResult(new PagedResultDto<AuditLogDto>(Array.Empty<AuditLogDto>(), 1, 20, 0, 1));

        public Task WriteAsync(string action, string entityType, string details, bool isSuccess, Guid? employeeId = null, string? username = null, Guid? branchId = null, Guid? entityId = null, CancellationToken cancellationToken = default)
            => Task.CompletedTask;
    }
}
