using CoffeeChainManagement.Application.DTOs.Promotions;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// PromotionsController cung cap CRUD khuyen mai va action-level authorization trong service.
[ApiController]
[Authorize]
[Route("api/promotions")]
public sealed class PromotionsController(IPromotionService promotionService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => await ExecuteAsync(() => promotionService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            var promotion = await promotionService.GetByIdAsync(id, cancellationToken);
            return promotion is null ? NotFound() : Ok(promotion);
        });

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPromotionRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await promotionService.CreateAsync(request, cancellationToken)));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertPromotionRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await promotionService.UpdateAsync(id, request, cancellationToken)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            await promotionService.DeleteAsync(id, cancellationToken);
            return NoContent();
        });

    private async Task<IActionResult> ExecuteAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return Ok(await action());
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    private async Task<IActionResult> ExecuteAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (KeyNotFoundException exception)
        {
            return NotFound(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
