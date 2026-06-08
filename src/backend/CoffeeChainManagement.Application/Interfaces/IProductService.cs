using CoffeeChainManagement.Application.DTOs.Products;

namespace CoffeeChainManagement.Application.Interfaces;

// IProductService gom use case doc menu san pham cho web admin.
public interface IProductService
{
    Task<IReadOnlyCollection<ProductSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
