using System;
using System.Collections.Generic;

namespace CoffeeChainManagement.Application.DTOs.Recipe;

public sealed record RecipeDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Instructions { get; init; } = string.Empty;
    public List<RecipeIngredientDto> Ingredients { get; init; } = new();
}
