using CoffeeChainManagement.Application.DTOs.Sales;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

[ApiController]
[Route("api/sales")]
[Authorize] // Require authentication (Cashier or Manager)
public class SalesController(ISaleOrderService saleOrderService) : ControllerBase
{
    [HttpPost("sync")]
    public async Task<IActionResult> SyncOrders([FromBody] SyncOrdersRequest request, CancellationToken cancellationToken)
    {
        if (request == null || request.Orders == null || request.Orders.Count == 0)
        {
            return BadRequest("No orders to sync.");
        }

        try
        {
            int syncedCount = await saleOrderService.SyncOrdersAsync(request, cancellationToken);
            return Ok(new { Message = $"Successfully synced {syncedCount} orders." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}
