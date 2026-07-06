namespace CoffeeChainManagement.Application.DTOs.Dashboard;

// KpiSummaryDto dua du lieu tong quan cho dashboard quan tri.
public sealed record KpiSummaryDto(
    decimal DailyRevenue,
    decimal MonthlyRevenue,
    decimal DailyInventoryExpense,
    decimal MonthlyInventoryExpense,
    decimal DailyNetRevenue,
    decimal MonthlyNetRevenue,
    int TotalOrders,
    int ActiveBranches,
    int LowStockAlerts);
