namespace CoffeeChainManagement.Application.DTOs.Reports;

// ReportProductRevenueDto luu mon ban chay theo so luong va doanh thu.
public sealed record ReportProductRevenueDto(Guid? ProductId, string ProductName, int Quantity, decimal Revenue);
