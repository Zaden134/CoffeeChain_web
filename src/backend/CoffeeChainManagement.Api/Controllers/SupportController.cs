using System.Security.Claims;
using CoffeeChainManagement.Application.DTOs.Support;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/support")]
public sealed class SupportController(IAuditLogService auditLogService) : ControllerBase
{
    [HttpPost("requests")]
    public async Task<IActionResult> Create([FromBody] CreateSupportRequestDto request, CancellationToken cancellationToken)
    {
        var userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedUserId)
            ? parsedUserId
            : (Guid?)null;
        var username = User.FindFirstValue(ClaimTypes.Name);

        await auditLogService.WriteAsync(
            "SUPPORT_REQUEST_CREATE",
            "SupportRequest",
            $"{request.Subject.Trim()}: {request.Message.Trim()}",
            true,
            userId,
            username,
            entityId: null,
            cancellationToken: cancellationToken);

        return Ok(new { message = "Support request received." });
    }
}
