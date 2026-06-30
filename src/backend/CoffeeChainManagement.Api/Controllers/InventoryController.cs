using CoffeeChainManagement.Application.DTOs.Inventory;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// InventoryController cung cap CRUD ton kho va lookup ingredient cho form.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager,WarehouseStaff")]
[Route("api/inventory")]
public sealed class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => await ExecuteAsync(() => inventoryService.GetAllAsync(cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            var item = await inventoryService.GetByIdAsync(id, cancellationToken);
            return item is null ? NotFound() : Ok(item);
        });

    [HttpGet("ingredients")]
    public async Task<IActionResult> GetIngredientLookups(CancellationToken cancellationToken)
        => await ExecuteAsync(() => inventoryService.GetIngredientLookupsAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertInventoryItemRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await inventoryService.CreateAsync(request, cancellationToken)));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertInventoryItemRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await inventoryService.UpdateAsync(id, request, cancellationToken)));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            await inventoryService.DeleteAsync(id, cancellationToken);
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
