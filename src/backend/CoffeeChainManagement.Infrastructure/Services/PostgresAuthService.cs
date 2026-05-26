using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CoffeeChainManagement.Application.DTOs.Auth;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Infrastructure.Auth;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresAuthService xac thuc user bang PostgreSQL va phat JWT cho frontend.
internal sealed class PostgresAuthService(
    CoffeeChainDbContext dbContext,
    IPasswordHasher<Employee> passwordHasher,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedUsername = request.Username.Trim().ToLowerInvariant();

        var user = await dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(
                employee => employee.IsActive && employee.Username.ToLower() == normalizedUsername,
                cancellationToken);

        if (user is null)
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return CreateAuthResponse(user);
    }

    public async Task<UserProfileDto?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Employees
            .AsNoTracking()
            .SingleOrDefaultAsync(employee => employee.Id == userId && employee.IsActive, cancellationToken);

        return user is null ? null : MapUser(user);
    }

    private AuthResponseDto CreateAuthResponse(Employee user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.BranchId.HasValue)
        {
            claims.Add(new Claim("branch_id", user.BranchId.Value.ToString()));
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AuthResponseDto(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAtUtc,
            MapUser(user));
    }

    private static UserProfileDto MapUser(Employee user)
        => new(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Role.ToString(),
            user.BranchId);
}
