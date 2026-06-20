using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Recipe;

public sealed record UpsertRecipeIngredientDto
{
    [Required]
    public Guid IngredientId { get; init; }

    [Range(1, int.MaxValue)]
    public int RequiredQuantity { get; init; }
}

public sealed record UpsertRecipeRequestDto
{
    [Required]
    public Guid ProductId { get; init; }

    [MaxLength(2000)]
    public string Instructions { get; init; } = string.Empty;

    public List<UpsertRecipeIngredientDto> Ingredients { get; init; } = new();
}
