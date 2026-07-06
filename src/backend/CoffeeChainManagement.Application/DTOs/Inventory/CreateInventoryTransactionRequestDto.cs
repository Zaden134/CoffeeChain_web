using System;
using System.ComponentModel.DataAnnotations;
using CoffeeChainManagement.Domain.Entities;

namespace CoffeeChainManagement.Application.DTOs.Inventory;

public sealed record CreateInventoryTransactionRequestDto
{
    [Required]
    public Guid IngredientId { get; init; }

    [Required]
    public Guid BranchId { get; init; }

    [Required]
    public TransactionType Type { get; init; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Range(0, double.MaxValue)]
    public decimal UnitCost { get; init; }

    [MaxLength(50)]
    public string ReferenceNumber { get; init; } = string.Empty;

    [MaxLength(1000)]
    public string Notes { get; init; } = string.Empty;
}
