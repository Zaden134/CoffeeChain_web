namespace CoffeeChainManagement.Application.DTOs.Dashboard;

// KpiSummaryDto dua du lieu tong quan cho dashboard quan tri.
public sealed record KpiSummaryDto(
    decimal DailyRevenue,
    decimal MonthlyRevenue,
    int TotalOrders,
    int ActiveBranches,
    int LowStockAlerts);
