using CoffeeChainManagement.Domain.Common;
using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Domain.Entities;

// SaleOrder dai dien hoa don POS, la diem noi giua ban hang, kho va bao cao doanh thu.
public sealed class SaleOrder : BaseEntity
{
    public required Guid BranchId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required PaymentMethod PaymentMethod { get; init; }
    public required OrderStatus Status { get; init; }
    public required IReadOnlyCollection<SaleOrderItem> Items { get; init; }
    public decimal TotalAmount => Items.Sum(item => item.LineTotal);
}
