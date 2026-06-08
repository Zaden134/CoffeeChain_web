using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// DashboardController tra du lieu tong quan cho trang admin Angular.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager,Cashier")]
[Route("api/dashboard")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview(CancellationToken cancellationToken)
    {
        var data = await dashboardService.GetOverviewAsync(cancellationToken);
        return Ok(data);
    }
}
