using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Ingredient la nguyen lieu goc de theo doi dinh muc va canh bao sap het hang.
public sealed class Ingredient : BaseEntity
{
    public required string Name { get; init; }
    public required string Unit { get; init; }
    public required decimal ReorderLevel { get; init; }
}
