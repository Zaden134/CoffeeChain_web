using CoffeeChainManagement.Application.DTOs.Dashboard;

namespace CoffeeChainManagement.Application.Interfaces;

// IDashboardService dinh nghia hop dong lay dashboard de API va Infrastructure tach roi ro rang.
public interface IDashboardService
{
    Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default);
}
