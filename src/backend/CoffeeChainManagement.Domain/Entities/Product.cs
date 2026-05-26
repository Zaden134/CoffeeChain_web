using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Product mo ta mon trong menu, duoc dung cho ca web quan tri va POS.
public sealed class Product : BaseEntity
{
    public required string Sku { get; init; }
    public required string Name { get; init; }
    public required string Category { get; init; }
    public required decimal Price { get; init; }
    public required string ImageUrl { get; init; }
    public bool IsAvailable { get; init; } = true;
}
