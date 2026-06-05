using CoffeeChainManagement.Application.DTOs.Reports;

namespace CoffeeChainManagement.Application.Interfaces;

// IReportService cung cap bao cao loc theo thoi gian/chi nhanh va export file.
public interface IReportService
{
    Task<SalesReportDto> GetSalesReportAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    Task<ReportExportResultDto> ExportSalesReportAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? branchId,
        string format,
        CancellationToken cancellationToken = default);
}
