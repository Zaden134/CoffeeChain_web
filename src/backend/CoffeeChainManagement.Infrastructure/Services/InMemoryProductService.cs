using CoffeeChainManagement.Application.DTOs.Products;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Infrastructure.Persistence;

namespace CoffeeChainManagement.Infrastructure.Services;

// InMemoryProductService giu API menu chay duoc truoc khi co bang Products trong database that.
internal sealed class InMemoryProductService : IProductService
{
    public Task<IReadOnlyCollection<ProductSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(InMemorySeedData.Products);
}
