using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Product mo ta mon trong menu, duoc dung cho ca web quan tri va POS.
public sealed class Product : BaseEntity
{
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required decimal Price { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
}
