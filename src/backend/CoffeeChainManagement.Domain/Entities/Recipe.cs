using System;
using System.Collections.Generic;
using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

public sealed class Recipe : BaseEntity
{
    public Guid ProductId { get; set; }
    public string Instructions { get; set; } = string.Empty;

    public Product? Product { get; set; }
    public ICollection<RecipeIngredient> Ingredients { get; set; } = new List<RecipeIngredient>();
}
