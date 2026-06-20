using System;

namespace CoffeeChainManagement.Application.DTOs.Recipe;

public sealed record RecipeIngredientDto
{
    public Guid Id { get; init; }
    public Guid RecipeId { get; init; }
    public Guid IngredientId { get; init; }
    public string IngredientName { get; init; } = string.Empty;
    public string IngredientUnit { get; init; } = string.Empty;
    public int RequiredQuantity { get; init; }
}
