using CoffeeChainManagement.Application.DTOs.Reports;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresReportService tong hop du lieu bao cao va ho tro export xlsx/pdf.
internal sealed class PostgresReportService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser) : IReportService
{
    public async Task<SalesReportDto> GetSalesReportAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? branchId,
        CancellationToken cancellationToken = default)
    {
        var effectiveBranchId = ResolveBranchId(branchId);
        var orders = await BuildOrderQuery(fromDate, toDate, effectiveBranchId)
            .Include(order => order.Items)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        var inventoryTransactions = await dbContext.InventoryTransactions
            .AsNoTracking()
            .Where(transaction => !effectiveBranchId.HasValue || transaction.BranchId == effectiveBranchId.Value)
            .Where(transaction => !fromDate.HasValue || transaction.CreatedAtUtc >= fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))
            .Where(transaction => !toDate.HasValue || transaction.CreatedAtUtc <= toDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc))
            .ToListAsync(cancellationToken);

        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        var products = await dbContext.Products.AsNoTracking().ToDictionaryAsync(product => product.Id, product => product.Name, cancellationToken);
        var inventoryExpenseByBranch = inventoryTransactions
            .GroupBy(transaction => transaction.BranchId)
            .ToDictionary(group => group.Key, group => group.Sum(transaction => transaction.TransactionAmount));
        var inventoryExpenseByDay = inventoryTransactions
            .GroupBy(transaction => DateOnly.FromDateTime(transaction.CreatedAtUtc))
            .ToDictionary(group => group.Key, group => group.Sum(transaction => transaction.TransactionAmount));

        var ordersByBranch = orders
            .GroupBy(order => order.BranchId)
            .ToDictionary(group => group.Key, group => group.ToArray());
        var branchKeys = ordersByBranch.Keys
            .Concat(inventoryExpenseByBranch.Keys)
            .Distinct()
            .ToArray();
        var branchRevenue = branchKeys
            .Select(branchId =>
            {
                var branchOrders = ordersByBranch.GetValueOrDefault(branchId) ?? [];
                return new ReportBranchRevenueDto(
                    branchId,
                    branches.GetValueOrDefault(branchId, "Khong ro chi nhanh"),
                    branchOrders.Sum(order => order.Items.Sum(item => item.LineTotal)) + inventoryExpenseByBranch.GetValueOrDefault(branchId),
                    branchOrders.Length);
            })
            .OrderByDescending(item => item.Revenue)
            .ToArray();

        var productRevenue = orders
            .SelectMany(order => order.Items)
            .GroupBy(item => new { item.ProductId, item.ProductName })
            .Select(group => new ReportProductRevenueDto(
                group.Key.ProductId,
                group.Key.ProductName,
                group.Sum(item => item.Quantity),
                group.Sum(item => item.LineTotal)))
            .OrderByDescending(item => item.Quantity)
            .ToArray();

        var ordersByDay = orders
            .GroupBy(order => DateOnly.FromDateTime(order.CreatedAtUtc))
            .ToDictionary(group => group.Key, group => group.ToArray());
        var dayKeys = ordersByDay.Keys
            .Concat(inventoryExpenseByDay.Keys)
            .Distinct()
            .ToArray();
        var dailyRevenue = dayKeys
            .Select(date =>
            {
                var dayOrders = ordersByDay.GetValueOrDefault(date) ?? [];
                return new ReportDailyRevenueDto(
                    date,
                    dayOrders.Sum(order => order.Items.Sum(item => item.LineTotal)) + inventoryExpenseByDay.GetValueOrDefault(date),
                    dayOrders.Length);
            })
            .OrderBy(item => item.Date)
            .ToArray();

        var totalRevenue = orders.Sum(order => order.Items.Sum(item => item.LineTotal));
        var inventoryExpense = inventoryTransactions.Sum(transaction => transaction.TransactionAmount);
        var netRevenue = totalRevenue + inventoryExpense;
        var totalOrders = orders.Count;
        var averageOrderValue = totalOrders == 0 ? 0 : totalRevenue / totalOrders;

        var inventoryQuery = dbContext.InventoryItems.AsNoTracking();
        if (effectiveBranchId.HasValue)
        {
            inventoryQuery = inventoryQuery.Where(item => item.BranchId == effectiveBranchId.Value);
        }

        var lowStockItems = await (
            from inventory in inventoryQuery
            join ingredient in dbContext.Ingredients.AsNoTracking() on inventory.IngredientId equals ingredient.Id
            where inventory.InStockQuantity <= ingredient.ReorderLevel
            select inventory.Id).CountAsync(cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var activePromotions = await dbContext.Promotions.AsNoTracking().CountAsync(
            promotion => promotion.IsActive
                && promotion.StartDate <= today
                && promotion.EndDate >= today
                && (!effectiveBranchId.HasValue || promotion.BranchId == null || promotion.BranchId == effectiveBranchId.Value),
            cancellationToken);
        var pendingRecruitments = await dbContext.RecruitmentRequests.AsNoTracking().CountAsync(request =>
            request.Status == RecruitmentRequestStatus.Pending && (!effectiveBranchId.HasValue || request.BranchId == effectiveBranchId.Value), cancellationToken);

        return new SalesReportDto(
            fromDate,
            toDate,
            effectiveBranchId,
            effectiveBranchId.HasValue && branches.TryGetValue(effectiveBranchId.Value, out var branchName) ? branchName : null,
            totalRevenue,
            inventoryExpense,
            netRevenue,
            totalOrders,
            averageOrderValue,
            await dbContext.Branches.AsNoTracking().CountAsync(
                branch => branch.IsActive && (!effectiveBranchId.HasValue || branch.Id == effectiveBranchId.Value),
                cancellationToken),
            lowStockItems,
            activePromotions,
            pendingRecruitments,
            dailyRevenue,
            branchRevenue,
            productRevenue);
    }

    public async Task<ReportExportResultDto> ExportSalesReportAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        Guid? branchId,
        string format,
        CancellationToken cancellationToken = default)
    {
        var report = await GetSalesReportAsync(fromDate, toDate, branchId, cancellationToken);

        return format.Trim().ToLowerInvariant() switch
        {
            "pdf" => new ReportExportResultDto(
                BuildFileName(report, "pdf"),
                "application/pdf",
                BuildPdf(report)),
            _ => new ReportExportResultDto(
                BuildFileName(report, "xlsx"),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                BuildExcel(report))
        };
    }

    private IQueryable<Domain.Entities.SaleOrder> BuildOrderQuery(DateOnly? fromDate, DateOnly? toDate, Guid? branchId)
    {
        var query = dbContext.SaleOrders.Where(order => order.Status == OrderStatus.Paid);

        if (branchId.HasValue)
        {
            query = query.Where(order => order.BranchId == branchId.Value);
        }

        if (fromDate.HasValue)
        {
            var fromUtc = fromDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            query = query.Where(order => order.CreatedAtUtc >= fromUtc);
        }

        if (toDate.HasValue)
        {
            var toUtc = toDate.Value.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);
            query = query.Where(order => order.CreatedAtUtc <= toUtc);
        }

        return query.OrderByDescending(order => order.CreatedAtUtc);
    }

    private Guid? ResolveBranchId(Guid? requestedBranchId)
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to access reports.");
        }

        if (currentUser.Role == UserRole.Administrator)
        {
            return requestedBranchId;
        }

        if (currentUser.Role == UserRole.BranchManager && currentUser.BranchId.HasValue)
        {
            if (requestedBranchId.HasValue && requestedBranchId.Value != currentUser.BranchId.Value)
            {
                throw new UnauthorizedAccessException("Branch managers can only access reports for their own branch.");
            }

            return currentUser.BranchId.Value;
        }

        throw new UnauthorizedAccessException("You do not have permission to access reports.");
    }

    private static byte[] BuildExcel(SalesReportDto report)
    {
        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Sales Report");

        sheet.Cell(1, 1).Value = "Sales Report";
        sheet.Cell(2, 1).Value = "From";
        sheet.Cell(2, 2).Value = report.FromDate?.ToString("yyyy-MM-dd") ?? "All";
        sheet.Cell(3, 1).Value = "To";
        sheet.Cell(3, 2).Value = report.ToDate?.ToString("yyyy-MM-dd") ?? "All";
        sheet.Cell(4, 1).Value = "Branch";
        sheet.Cell(4, 2).Value = report.BranchName ?? "All branches";
        sheet.Cell(5, 1).Value = "Total Revenue";
        sheet.Cell(5, 2).Value = report.TotalRevenue;
        sheet.Cell(6, 1).Value = "Total Orders";
        sheet.Cell(6, 2).Value = report.TotalOrders;
        sheet.Cell(7, 1).Value = "Inventory Expense";
        sheet.Cell(7, 2).Value = report.InventoryExpense;
        sheet.Cell(8, 1).Value = "Net Revenue";
        sheet.Cell(8, 2).Value = report.NetRevenue;

        var row = 10;
        sheet.Cell(row, 1).Value = "Daily Revenue";
        row++;
        sheet.Cell(row, 1).Value = "Date";
        sheet.Cell(row, 2).Value = "Revenue";
        sheet.Cell(row, 3).Value = "Orders";
        row++;
        foreach (var daily in report.DailyRevenue)
        {
            sheet.Cell(row, 1).Value = daily.Date.ToString("yyyy-MM-dd");
            sheet.Cell(row, 2).Value = daily.Revenue;
            sheet.Cell(row, 3).Value = daily.Orders;
            row++;
        }

        row += 2;
        sheet.Cell(row, 1).Value = "Branch Revenue";
        row++;
        sheet.Cell(row, 1).Value = "Branch";
        sheet.Cell(row, 2).Value = "Revenue";
        sheet.Cell(row, 3).Value = "Orders";
        row++;
        foreach (var branch in report.BranchRevenue)
        {
            sheet.Cell(row, 1).Value = branch.BranchName;
            sheet.Cell(row, 2).Value = branch.Revenue;
            sheet.Cell(row, 3).Value = branch.Orders;
            row++;
        }

        row += 2;
        sheet.Cell(row, 1).Value = "Product Revenue";
        row++;
        sheet.Cell(row, 1).Value = "Product";
        sheet.Cell(row, 2).Value = "Quantity";
        sheet.Cell(row, 3).Value = "Revenue";
        row++;
        foreach (var product in report.ProductRevenue)
        {
            sheet.Cell(row, 1).Value = product.ProductName;
            sheet.Cell(row, 2).Value = product.Quantity;
            sheet.Cell(row, 3).Value = product.Revenue;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static byte[] BuildPdf(SalesReportDto report)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(32);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(style => style.FontSize(10));

                page.Content().Column(column =>
                {
                    column.Item().Text("Sales Report").FontSize(20).SemiBold();
                    column.Item().Text($"From: {report.FromDate?.ToString("yyyy-MM-dd") ?? "All"}");
                    column.Item().Text($"To: {report.ToDate?.ToString("yyyy-MM-dd") ?? "All"}");
                    column.Item().Text($"Branch: {report.BranchName ?? "All branches"}");
                    column.Item().Text($"Revenue: {report.TotalRevenue:N0}");
                    column.Item().Text($"Inventory expense: {report.InventoryExpense:N0}");
                    column.Item().Text($"Net revenue: {report.NetRevenue:N0}");
                    column.Item().Text($"Orders: {report.TotalOrders}");

                    column.Item().PaddingTop(12).Text("Daily Revenue").SemiBold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Date").SemiBold();
                            header.Cell().Text("Revenue").SemiBold();
                            header.Cell().Text("Orders").SemiBold();
                        });

                        foreach (var daily in report.DailyRevenue)
                        {
                            table.Cell().Text(daily.Date.ToString("yyyy-MM-dd"));
                            table.Cell().Text(daily.Revenue.ToString("N0"));
                            table.Cell().Text(daily.Orders.ToString());
                        }
                    });
                });
            });
        });

        return document.GeneratePdf();
    }

    private static string BuildFileName(SalesReportDto report, string extension)
    {
        var branch = Slugify(report.BranchName ?? "toan-he-thong");
        var from = report.FromDate?.ToString("yyyyMMdd") ?? "tu-dau";
        var to = report.ToDate?.ToString("yyyyMMdd") ?? DateTime.UtcNow.ToString("yyyyMMdd");
        return $"bao-cao-doanh-thu_{branch}_{from}_{to}.{extension}";
    }

    private static string Slugify(string value)
    {
        var chars = value
            .Trim()
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray();

        return string.Join('-', new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
    }
}
