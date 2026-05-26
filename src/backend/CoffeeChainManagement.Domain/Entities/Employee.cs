using CoffeeChainManagement.Domain.Common;
using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Domain.Entities;

// Employee gom thong tin nhan vien va vai tro de sau nay mo rong phan quyen.
public sealed class Employee : BaseEntity
{
    public required Guid BranchId { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required UserRole Role { get; init; }
    public bool IsActive { get; init; } = true;
}
