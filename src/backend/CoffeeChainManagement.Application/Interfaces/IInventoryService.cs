using CoffeeChainManagement.Application.DTOs.Common;
using CoffeeChainManagement.Application.DTOs.Inventory;

namespace CoffeeChainManagement.Application.Interfaces;

// IInventoryService dinh nghia CRUD ton kho va lookup ingredient/branch cho form.
public interface IInventoryService
{
    Task<IReadOnlyCollection<InventoryItemDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<InventoryItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryItemDto> CreateAsync(UpsertInventoryItemRequestDto request, CancellationToken cancellationToken = default);
    Task<InventoryItemDto> UpdateAsync(Guid id, UpsertInventoryItemRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<LookupDto>> GetIngredientLookupsAsync(CancellationToken cancellationToken = default);
}
