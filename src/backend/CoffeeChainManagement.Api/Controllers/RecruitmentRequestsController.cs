using CoffeeChainManagement.Application.DTOs.Recruitment;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// RecruitmentRequestsController xu ly manager gui yeu cau va admin phe duyet.
[ApiController]
[Authorize(Roles = "Administrator,BranchManager")]
[Route("api/recruitment-requests")]
public sealed class RecruitmentRequestsController(IRecruitmentRequestService recruitmentRequestService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => await ExecuteAsync(() => recruitmentRequestService.GetAllAsync(cancellationToken));

    [HttpPost]
    [Authorize(Roles = "BranchManager")]
    public async Task<IActionResult> Create([FromBody] CreateRecruitmentRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await recruitmentRequestService.CreateAsync(request, cancellationToken)));

    [HttpPut("{id:guid}/review")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Review(Guid id, [FromBody] ReviewRecruitmentRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => Ok(await recruitmentRequestService.ReviewAsync(id, request, cancellationToken)));

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
}
