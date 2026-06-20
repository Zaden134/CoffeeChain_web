using System;
using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

public enum TransactionType
{
    Import = 1,
    Export = 2,
    Adjustment = 3
}

public sealed class InventoryTransaction : BaseEntity
{
    public Guid IngredientId { get; set; }
    public Guid BranchId { get; set; }
    public TransactionType Type { get; set; }
    public int Quantity { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public Guid CreatedBy { get; set; }

    public Ingredient? Ingredient { get; set; }
    public Branch? Branch { get; set; }
}
