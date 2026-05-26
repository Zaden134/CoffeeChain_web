using CoffeeChainManagement.Application.DTOs.Dashboard;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Infrastructure.Persistence;

namespace CoffeeChainManagement.Infrastructure.Services;

// InMemoryDashboardService la implementation tam de frontend co endpoint test ngay.
internal sealed class InMemoryDashboardService : IDashboardService
{
    public Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(InMemorySeedData.DashboardOverview);
}
