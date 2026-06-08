namespace CoffeeChainManagement.Application.DTOs.Inventory;

// InventoryItemDto la du lieu ton kho theo chi nhanh cho giao dien va form.
public sealed record InventoryItemDto(
    Guid Id,
    Guid BranchId,
    string BranchName,
    Guid IngredientId,
    string IngredientName,
    string Unit,
    decimal ReorderLevel,
    decimal InStockQuantity,
    decimal ReservedQuantity,
    bool IsLowStock);
