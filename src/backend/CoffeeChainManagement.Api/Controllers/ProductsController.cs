using CoffeeChainManagement.Application.DTOs.Products;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// ProductsController phuc vu menu, mon ban chay va du lieu hien thi danh muc.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager,Cashier")]
[Route("api/products")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await productService.GetAllAsync(cancellationToken);
        return Ok(data);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            var product = await productService.GetByIdAsync(id, cancellationToken);
            return product is null ? NotFound() : Ok(product);
        });

    [Authorize(Roles = "Administrator,BranchManager")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertProductRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await productService.CreateAsync(request, cancellationToken)));

    [Authorize(Roles = "Administrator,BranchManager")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertProductRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await productService.UpdateAsync(id, request, cancellationToken)));

    [Authorize(Roles = "Administrator,BranchManager")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        => await ExecuteAsync(async () =>
        {
            await productService.DeleteAsync(id, cancellationToken);
            return NoContent();
        });

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
