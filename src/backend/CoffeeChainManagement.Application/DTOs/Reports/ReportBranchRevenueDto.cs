namespace CoffeeChainManagement.Application.DTOs.Reports;

// ReportBranchRevenueDto luu tong doanh thu theo chi nhanh.
public sealed record ReportBranchRevenueDto(Guid BranchId, string BranchName, decimal Revenue, int Orders);
