using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CoffeeChainManagement.Infrastructure.Auth;

// CurrentUserContext doc thong tin user hien tai tu JWT claims.
public sealed class CurrentUserContext(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid UserId => Guid.TryParse(User?.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
        ? userId
        : Guid.Empty;

    public string? Username => User?.FindFirstValue(ClaimTypes.Name) ?? User?.FindFirstValue(JwtRegisteredClaimNames.UniqueName);

    public Guid? BranchId => Guid.TryParse(User?.FindFirstValue("branch_id"), out var branchId)
        ? branchId
        : null;

    public UserRole Role => Enum.TryParse<UserRole>(User?.FindFirstValue(ClaimTypes.Role), out var role)
        ? role
        : default;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
