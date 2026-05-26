using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// InventoryItem gan nguyen lieu voi tung chi nhanh de quan ly ton kho theo cua hang.
public sealed class InventoryItem : BaseEntity
{
    public required Guid BranchId { get; init; }
    public required Guid IngredientId { get; init; }
    public required decimal InStockQuantity { get; init; }
    public required decimal ReservedQuantity { get; init; }
}
