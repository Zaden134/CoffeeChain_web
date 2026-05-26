using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Branch dai dien mot cua hang trong chuoi, la tam cua luong quan ly doanh thu va ton kho.
public sealed class Branch : BaseEntity
{
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required string Address { get; init; }
    public required string ManagerName { get; init; }
    public bool IsActive { get; init; } = true;
}
