using CoffeeChainManagement.Domain.Common;
using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Domain.Entities;

// Employee gom thong tin nhan vien va vai tro de sau nay mo rong phan quyen.
public sealed class Employee : BaseEntity
{
    public Guid? BranchId { get; set; }
    public required string Username { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public required UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
}
