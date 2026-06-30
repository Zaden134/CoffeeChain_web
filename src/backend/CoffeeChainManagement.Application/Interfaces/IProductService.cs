using CoffeeChainManagement.Application.DTOs.Products;

namespace CoffeeChainManagement.Application.Interfaces;

// IProductService gom use case doc menu san pham cho web admin.
public interface IProductService
{
    Task<IReadOnlyCollection<ProductSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductSummaryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductSummaryDto> CreateAsync(UpsertProductRequestDto request, CancellationToken cancellationToken = default);
    Task<ProductSummaryDto> UpdateAsync(Guid id, UpsertProductRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
