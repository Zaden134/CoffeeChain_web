using System;
using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

public sealed class RecipeIngredient : BaseEntity
{
    public Guid RecipeId { get; set; }
    public Guid IngredientId { get; set; }
    public int RequiredQuantity { get; set; }

    public Recipe? Recipe { get; set; }
    public Ingredient? Ingredient { get; set; }
}
