using CoffeeChainManagement.Domain.Common;
using CoffeeChainManagement.Domain.Enums;

namespace CoffeeChainManagement.Domain.Entities;

// SaleOrder dai dien hoa don POS, la diem noi giua ban hang, kho va bao cao doanh thu.
public sealed class SaleOrder : BaseEntity
{
    public required Guid BranchId { get; set; }
    public required Guid EmployeeId { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
    public required OrderStatus Status { get; set; }
    public string? PromotionCode { get; set; }
    public decimal DiscountAmount { get; set; }
    public List<SaleOrderItem> Items { get; set; } = [];
    public decimal TotalAmount => Items.Sum(item => item.LineTotal) - DiscountAmount;
}
