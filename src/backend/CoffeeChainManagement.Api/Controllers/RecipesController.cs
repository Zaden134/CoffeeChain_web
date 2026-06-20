using System;
using System.Threading;
using System.Threading.Tasks;
using CoffeeChainManagement.Application.DTOs.Recipe;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = "Administrator,BranchManager")]
[Route("api/recipes")]
public sealed class RecipesController(IRecipeService recipeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await recipeService.GetAllAsync(cancellationToken));

    [HttpGet("product/{productId:guid}")]
    public async Task<IActionResult> GetByProductId(Guid productId, CancellationToken cancellationToken)
    {
        var recipe = await recipeService.GetByProductIdAsync(productId, cancellationToken);
        return recipe == null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] UpsertRecipeRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await recipeService.UpsertAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await recipeService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
