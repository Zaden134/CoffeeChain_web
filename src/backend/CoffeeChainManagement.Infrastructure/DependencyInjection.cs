using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Infrastructure.Auth;
using CoffeeChainManagement.Infrastructure.Persistence;
using CoffeeChainManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeChainManagement.Infrastructure;

// DependencyInjection tap trung dang ky service de Program.cs gon va de bao tri.
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = configuration[$"{JwtOptions.SectionName}:Issuer"]
                ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
            options.Audience = configuration[$"{JwtOptions.SectionName}:Audience"]
                ?? throw new InvalidOperationException("Jwt:Audience is missing.");
            options.SigningKey = configuration[$"{JwtOptions.SectionName}:SigningKey"]
                ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

            if (int.TryParse(configuration[$"{JwtOptions.SectionName}:ExpiryMinutes"], out var expiryMinutes))
            {
                options.ExpiryMinutes = expiryMinutes;
            }
        });

        services.AddHttpContextAccessor();
        services.AddDbContext<CoffeeChainDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

        services.AddScoped<IPasswordHasher<Employee>, PasswordHasher<Employee>>();
        services.AddScoped<CoffeeChainDbSeeder>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();
        services.AddScoped<IAuditLogService, PostgresAuditLogService>();

        services.AddScoped<IDashboardService, PostgresDashboardService>();
        services.AddScoped<IBranchService, PostgresBranchService>();
        services.AddScoped<IProductService, PostgresProductService>();
        services.AddScoped<IAuthService, PostgresAuthService>();
        services.AddScoped<IReportService, PostgresReportService>();
        services.AddScoped<IEmployeeService, PostgresEmployeeService>();
        services.AddScoped<IInventoryService, PostgresInventoryService>();
        services.AddScoped<IPromotionService, PostgresPromotionService>();
        services.AddScoped<IRecruitmentRequestService, PostgresRecruitmentRequestService>();
        services.AddScoped<ISaleOrderService, PostgresSaleOrderService>();

        return services;
    }

    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.MigrateAsync(cancellationToken);
        await initializer.SeedAsync(cancellationToken);
    }
}
