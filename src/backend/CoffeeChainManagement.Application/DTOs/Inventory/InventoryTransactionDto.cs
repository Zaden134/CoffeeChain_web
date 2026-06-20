using System;
using CoffeeChainManagement.Domain.Entities;

namespace CoffeeChainManagement.Application.DTOs.Inventory;

public sealed record InventoryTransactionDto
{
    public Guid Id { get; init; }
    public Guid IngredientId { get; init; }
    public string IngredientName { get; init; } = string.Empty;
    public Guid BranchId { get; init; }
    public string BranchName { get; init; } = string.Empty;
    public TransactionType Type { get; init; }
    public int Quantity { get; init; }
    public string ReferenceNumber { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public Guid CreatedBy { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
