using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Inventory;

// UpsertInventoryItemRequestDto ho tro tao/sua ton kho va ingredient neu can.
public sealed class UpsertInventoryItemRequestDto
{
    public Guid? IngredientId { get; init; }

    [StringLength(150)]
    public string? IngredientName { get; init; }

    [StringLength(20)]
    public string? Unit { get; init; }

    [Range(0, 999999)]
    public decimal ReorderLevel { get; init; }

    [Required]
    public Guid BranchId { get; init; }

    [Range(0, 999999)]
    public decimal InStockQuantity { get; init; }

    [Range(0, 999999)]
    public decimal ReservedQuantity { get; init; }
}
