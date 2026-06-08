using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Application.Interfaces;

// ICurrentUserContext gom thong tin user tu JWT de service co the ap dung rule theo action.
public interface ICurrentUserContext
{
    Guid UserId { get; }
    string? Username { get; }
    Guid? BranchId { get; }
    UserRole Role { get; }
    bool IsAuthenticated { get; }
}
