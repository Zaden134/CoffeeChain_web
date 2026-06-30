using System.Security.Claims;
using CoffeeChainManagement.Application.DTOs.Auth;
using CoffeeChainManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeChainManagement.Api.Controllers;

// AuthController xu ly login JWT va tra profile user dang dang nhap.
[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.LoginAsync(request, cancellationToken);
        return response is null ? Unauthorized(new { message = "Invalid username or password." }) : Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request, CancellationToken cancellationToken)
    {
        var response = await authService.RefreshAsync(request, cancellationToken);
        return response is null ? Unauthorized(new { message = "Refresh token is invalid or expired." }) : Ok(response);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequestDto request, CancellationToken cancellationToken)
        => await authService.LogoutAsync(request, cancellationToken)
            ? NoContent()
            : NotFound(new { message = "Refresh token was not found." });

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var profile = await authService.GetByIdAsync(userId, cancellationToken);
        return profile is null ? Unauthorized() : Ok(profile);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            return await authService.ChangePasswordAsync(userId, request, cancellationToken)
                ? NoContent()
                : BadRequest(new { message = "Current password is invalid." });
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [Authorize(Roles = "Administrator")]
    [HttpPost("users/{employeeId:guid}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid employeeId, [FromBody] ResetPasswordRequestDto request, CancellationToken cancellationToken)
        => await ExecuteAdminAsync(async () =>
            await authService.ResetPasswordAsync(employeeId, request, cancellationToken)
                ? NoContent()
                : NotFound(new { message = "Employee was not found." }));

    [Authorize(Roles = "Administrator")]
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] bool activeOnly = false,
        CancellationToken cancellationToken = default)
        => await ExecuteAdminAsync(async () => Ok(await authService.GetSessionsAsync(new UserSessionQueryDto(page, pageSize, search, activeOnly), cancellationToken)));

    [Authorize(Roles = "Administrator")]
    [HttpPost("sessions/{sessionId:guid}/revoke")]
    public async Task<IActionResult> RevokeSession(Guid sessionId, CancellationToken cancellationToken)
        => await ExecuteAdminAsync(async () =>
            await authService.RevokeSessionAsync(sessionId, cancellationToken)
                ? NoContent()
                : NotFound(new { message = "Session was not found." }));

    private async Task<IActionResult> ExecuteAdminAsync(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
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
