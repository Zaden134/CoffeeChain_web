using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// ReportsController cung cap bao cao loc theo thoi gian/chi nhanh va export file.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager")]
[Route("api/reports")]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    [HttpGet("sales")]
    public async Task<IActionResult> GetSales(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? branchId,
        CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await reportService.GetSalesReportAsync(fromDate, toDate, branchId, cancellationToken)));

    [HttpGet("sales/export")]
    public async Task<IActionResult> ExportSales(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        [FromQuery] Guid? branchId,
        [FromQuery] string format = "xlsx",
        CancellationToken cancellationToken = default)
        => await ExecuteAsync(async () =>
        {
            var export = await reportService.ExportSalesReportAsync(fromDate, toDate, branchId, format, cancellationToken);
            return File(export.Content, export.ContentType, export.FileName);
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
    }
}
