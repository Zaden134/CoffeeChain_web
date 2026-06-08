using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// InventoryItem gan nguyen lieu voi tung chi nhanh de quan ly ton kho theo cua hang.
public sealed class InventoryItem : BaseEntity
{
    public required Guid BranchId { get; set; }
    public required Guid IngredientId { get; set; }
    public required decimal InStockQuantity { get; set; }
    public required decimal ReservedQuantity { get; set; }
}
