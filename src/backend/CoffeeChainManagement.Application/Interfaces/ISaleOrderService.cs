using CoffeeChainManagement.Application.DTOs.Sales;

namespace CoffeeChainManagement.Application.Interfaces;

public interface ISaleOrderService
{
    Task<int> SyncOrdersAsync(SyncOrdersRequest request, CancellationToken cancellationToken = default);
}
