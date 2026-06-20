using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CoffeeChainManagement.Application.DTOs.Inventory;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

[ApiController]
[Authorize(Roles = "Administrator,BranchManager")]
[Route("api/inventory-transactions")]
public sealed class InventoryTransactionsController(IInventoryTransactionService txService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await txService.GetAllAsync(cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryTransactionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var employeeIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(employeeIdClaim) || !Guid.TryParse(employeeIdClaim, out var employeeId))
            {
                return Unauthorized();
            }

            var result = await txService.CreateTransactionAsync(request, employeeId, cancellationToken);
            return Ok(result);
        }
        catch (System.Collections.Generic.KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
