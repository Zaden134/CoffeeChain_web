using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// BranchesController dat san endpoint quan ly chi nhanh cho phase web admin.
[ApiController]
[Route("api/branches")]
public sealed class BranchesController(IBranchService branchService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var data = await branchService.GetAllAsync(cancellationToken);
        return Ok(data);
    }
}
