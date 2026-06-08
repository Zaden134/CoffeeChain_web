namespace CoffeeChainManagement.Application.DTOs.Branches;

// BranchSummaryDto la du lieu gon de hien thi danh sach chi nhanh va doanh thu nhanh.
public sealed record BranchSummaryDto(
    Guid Id,
    string Code,
    string Name,
    string Address,
    string ManagerName,
    decimal RevenueToday,
    int LowStockItems,
    bool IsActive);
