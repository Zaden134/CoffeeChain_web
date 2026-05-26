using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// Branch dai dien mot cua hang trong chuoi, la tam cua luong quan ly doanh thu va ton kho.
public sealed class Branch : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public required string ManagerName { get; set; }
    public bool IsActive { get; set; } = true;
}
