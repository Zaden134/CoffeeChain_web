using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Ingredient la nguyen lieu goc de theo doi dinh muc va canh bao sap het hang.
public sealed class Ingredient : BaseEntity
{
    public required string Name { get; set; }
    public required string Unit { get; set; }
    public required decimal ReorderLevel { get; set; }
}
