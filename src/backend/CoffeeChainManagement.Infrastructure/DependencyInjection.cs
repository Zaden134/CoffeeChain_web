using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeChainManagement.Infrastructure;

// DependencyInjection tap trung dang ky service de Program.cs gon va de bao tri.
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDashboardService, InMemoryDashboardService>();
        services.AddSingleton<IBranchService, InMemoryBranchService>();
        services.AddSingleton<IProductService, InMemoryProductService>();

        return services;
    }
}
