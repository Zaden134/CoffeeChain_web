namespace CoffeeChainManagement.Application.DTOs.Reports;

// ReportExportResultDto gom file xuat ra de controller tra ve FileResult.
public sealed record ReportExportResultDto(string FileName, string ContentType, byte[] Content);
