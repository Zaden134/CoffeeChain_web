using CoffeeChainManagement.Application.DTOs.Dashboard;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresDashboardService tong hop KPI tu bang chi nhanh, ton kho va don hang.
internal sealed class PostgresDashboardService(
    CoffeeChainDbContext dbContext,
    IBranchService branchService,
    IProductService productService) : IDashboardService
{
    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var startOfDayUtc = nowUtc.Date;
        var startOfMonthUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var paidOrders = await dbContext.SaleOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.Status == OrderStatus.Paid)
            .ToListAsync(cancellationToken);

        var lowStockAlerts = await (
            from inventory in dbContext.InventoryItems.AsNoTracking()
            join ingredient in dbContext.Ingredients.AsNoTracking() on inventory.IngredientId equals ingredient.Id
            where inventory.InStockQuantity <= ingredient.ReorderLevel
            select inventory.Id)
            .CountAsync(cancellationToken);

        var branches = await branchService.GetAllAsync(cancellationToken);
        var products = await productService.GetAllAsync(cancellationToken);

        var dailyRevenue = paidOrders
            .Where(order => order.CreatedAtUtc >= startOfDayUtc)
            .Sum(order => order.Items.Sum(item => item.LineTotal));

        var monthlyRevenue = paidOrders
            .Where(order => order.CreatedAtUtc >= startOfMonthUtc)
            .Sum(order => order.Items.Sum(item => item.LineTotal));

        return new DashboardOverviewDto(
            new KpiSummaryDto(
                dailyRevenue,
                monthlyRevenue,
                paidOrders.Count,
                branches.Count(branch => branch.IsActive),
                lowStockAlerts),
            branches.OrderByDescending(branch => branch.RevenueToday).Take(3).ToArray(),
            products.OrderByDescending(product => product.SoldQuantity).Take(3).ToArray());
    }
}
