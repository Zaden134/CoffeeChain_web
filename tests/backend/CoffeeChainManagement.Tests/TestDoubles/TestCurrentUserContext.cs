using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Tests.TestDoubles;

internal sealed class TestCurrentUserContext(Guid userId, string? username, Guid? branchId, UserRole role, bool isAuthenticated = true) : ICurrentUserContext
{
    public Guid UserId => userId;
    public string? Username => username;
    public Guid? BranchId => branchId;
    public UserRole Role => role;
    public bool IsAuthenticated => isAuthenticated;
}
