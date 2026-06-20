using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoffeeChainManagement.Application.DTOs.Recipe;

namespace CoffeeChainManagement.Application.Interfaces;

public interface IRecipeService
{
    Task<List<RecipeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RecipeDto?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<RecipeDto> UpsertAsync(UpsertRecipeRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
