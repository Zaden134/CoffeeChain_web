using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// BranchesController dat san endpoint quan ly chi nhanh cho phase web admin.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager")]
[Route("api/branches")]
public sealed class BranchesController(IBranchService branchService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await branchService.GetAllAsync(cancellationToken);
        return Ok(data);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpsertBranchRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var data = await branchService.UpdateAsync(id, request, cancellationToken);
            return data is null ? NotFound(new { message = "Branch was not found." }) : Ok(data);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
