namespace CoffeeChainManagement.Application.DTOs.Reports;

// ReportDailyRevenueDto luu tong doanh thu theo ngay trong khoang bao cao.
public sealed record ReportDailyRevenueDto(DateOnly Date, decimal Revenue, int Orders);
