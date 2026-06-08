namespace CoffeeChainManagement.Application.DTOs.Reports;

// SalesReportDto tong hop du lieu loc theo thoi gian va chi nhanh.
public sealed record SalesReportDto(
    DateOnly? FromDate,
    DateOnly? ToDate,
    Guid? BranchId,
    string? BranchName,
    decimal TotalRevenue,
    int TotalOrders,
    decimal AverageOrderValue,
    int ActiveBranches,
    int LowStockItems,
    int ActivePromotions,
    int PendingRecruitmentRequests,
    IReadOnlyCollection<ReportDailyRevenueDto> DailyRevenue,
    IReadOnlyCollection<ReportBranchRevenueDto> BranchRevenue,
    IReadOnlyCollection<ReportProductRevenueDto> ProductRevenue);
