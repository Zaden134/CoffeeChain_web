using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoffeeChainManagement.Application.DTOs.Recipe;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

internal sealed class PostgresRecipeService(CoffeeChainDbContext dbContext) : IRecipeService
{
    public async Task<List<RecipeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var recipes = await dbContext.Recipes
            .AsNoTracking()
            .Include(r => r.Product)
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .ToListAsync(cancellationToken);

        return recipes.Select(MapToDto).ToList();
    }

    public async Task<RecipeDto?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var recipe = await dbContext.Recipes
            .AsNoTracking()
            .Include(r => r.Product)
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.ProductId == productId, cancellationToken);

        return recipe == null ? null : MapToDto(recipe);
    }

    public async Task<RecipeDto> UpsertAsync(UpsertRecipeRequestDto request, CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FindAsync([request.ProductId], cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        var existingRecipe = await dbContext.Recipes
            .Include(r => r.Ingredients)
            .FirstOrDefaultAsync(r => r.ProductId == request.ProductId, cancellationToken);

        if (existingRecipe == null)
        {
            existingRecipe = new Recipe
            {
                ProductId = request.ProductId,
                Instructions = request.Instructions
            };
            dbContext.Recipes.Add(existingRecipe);
        }
        else
        {
            existingRecipe.Instructions = request.Instructions;
            dbContext.RecipeIngredients.RemoveRange(existingRecipe.Ingredients);
        }

        foreach (var ing in request.Ingredients)
        {
            var ingredientEntity = await dbContext.Ingredients.FindAsync([ing.IngredientId], cancellationToken)
                ?? throw new KeyNotFoundException($"Ingredient {ing.IngredientId} not found.");

            existingRecipe.Ingredients.Add(new RecipeIngredient
            {
                IngredientId = ing.IngredientId,
                RequiredQuantity = ing.RequiredQuantity
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetByProductIdAsync(existingRecipe.ProductId, cancellationToken) ?? throw new InvalidOperationException();
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var recipe = await dbContext.Recipes.FindAsync([id], cancellationToken)
            ?? throw new KeyNotFoundException("Recipe not found.");

        dbContext.Recipes.Remove(recipe);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static RecipeDto MapToDto(Recipe recipe)
    {
        return new RecipeDto
        {
            Id = recipe.Id,
            ProductId = recipe.ProductId,
            ProductName = recipe.Product?.Name ?? string.Empty,
            Instructions = recipe.Instructions,
            Ingredients = recipe.Ingredients.Select(ri => new RecipeIngredientDto
            {
                Id = ri.Id,
                RecipeId = ri.RecipeId,
                IngredientId = ri.IngredientId,
                IngredientName = ri.Ingredient?.Name ?? string.Empty,
                IngredientUnit = ri.Ingredient?.Unit ?? string.Empty,
                RequiredQuantity = ri.RequiredQuantity
            }).ToList()
        };
    }
}
