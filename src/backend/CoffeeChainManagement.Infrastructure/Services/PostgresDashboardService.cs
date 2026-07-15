using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.DTOs.Dashboard;
using CoffeeChainManagement.Application.DTOs.Products;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresDashboardService tong hop KPI tu du lieu PostgreSQL that theo pham vi quyen hien tai.
internal sealed class PostgresDashboardService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser) : IDashboardService
{
    public async Task<DashboardOverviewDto> GetOverviewAsync(CancellationToken cancellationToken = default)
    {
        var effectiveBranchId = ResolveBranchScope();
        var nowUtc = DateTime.UtcNow;
        var startOfDayUtc = nowUtc.Date;
        var startOfMonthUtc = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var paidOrdersQuery = dbContext.SaleOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.Status == OrderStatus.Paid);

        if (effectiveBranchId.HasValue)
        {
            paidOrdersQuery = paidOrdersQuery.Where(order => order.BranchId == effectiveBranchId.Value);
        }

        var paidOrders = await paidOrdersQuery.ToListAsync(cancellationToken);

        var inventoryTransactionsQuery = dbContext.InventoryTransactions.AsNoTracking();
        if (effectiveBranchId.HasValue)
        {
            inventoryTransactionsQuery = inventoryTransactionsQuery.Where(transaction => transaction.BranchId == effectiveBranchId.Value);
        }

        var inventoryTransactions = await inventoryTransactionsQuery.ToListAsync(cancellationToken);

        var dailyRevenue = paidOrders
            .Where(order => order.CreatedAtUtc >= startOfDayUtc)
            .Sum(order => order.Items.Sum(item => item.LineTotal));
        var monthlyRevenue = paidOrders
            .Where(order => order.CreatedAtUtc >= startOfMonthUtc)
            .Sum(order => order.Items.Sum(item => item.LineTotal));
        var dailyInventoryExpense = inventoryTransactions
            .Where(transaction => transaction.CreatedAtUtc >= startOfDayUtc)
            .Sum(transaction => transaction.TransactionAmount);
        var monthlyInventoryExpense = inventoryTransactions
            .Where(transaction => transaction.CreatedAtUtc >= startOfMonthUtc)
            .Sum(transaction => transaction.TransactionAmount);

        var branches = await BuildBranchSummariesAsync(paidOrders, inventoryTransactions, effectiveBranchId, startOfDayUtc, cancellationToken);
        var products = await BuildProductSummariesAsync(paidOrders, cancellationToken);

        return new DashboardOverviewDto(
            new KpiSummaryDto(
                dailyRevenue,
                monthlyRevenue,
                dailyInventoryExpense,
                monthlyInventoryExpense,
                dailyRevenue + dailyInventoryExpense,
                monthlyRevenue + monthlyInventoryExpense,
                paidOrders.Count,
                branches.Count(branch => branch.IsActive),
                branches.Sum(branch => branch.LowStockItems)),
            branches.OrderByDescending(branch => branch.RevenueToday).Take(5).ToArray(),
            products.OrderByDescending(product => product.SoldQuantity).Take(5).ToArray());
    }

    private async Task<IReadOnlyCollection<BranchSummaryDto>> BuildBranchSummariesAsync(
        IReadOnlyCollection<Domain.Entities.SaleOrder> paidOrders,
        IReadOnlyCollection<Domain.Entities.InventoryTransaction> inventoryTransactions,
        Guid? effectiveBranchId,
        DateTime startOfDayUtc,
        CancellationToken cancellationToken)
    {
        var branchQuery = dbContext.Branches.AsNoTracking();
        if (effectiveBranchId.HasValue)
        {
            branchQuery = branchQuery.Where(branch => branch.Id == effectiveBranchId.Value);
        }

        var branches = await branchQuery.OrderBy(branch => branch.Name).ToListAsync(cancellationToken);
        var todaySalesByBranch = paidOrders
            .Where(order => order.CreatedAtUtc >= startOfDayUtc)
            .GroupBy(order => order.BranchId)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(order => order.Items.Sum(item => item.LineTotal)));
        var todayInventoryByBranch = inventoryTransactions
            .Where(transaction => transaction.CreatedAtUtc >= startOfDayUtc)
            .GroupBy(transaction => transaction.BranchId)
            .ToDictionary(group => group.Key, group => group.Sum(transaction => transaction.TransactionAmount));
        var lowStockByBranch = await BuildLowStockLookupAsync(effectiveBranchId, cancellationToken);

        return branches
            .Select(branch => new BranchSummaryDto(
                branch.Id,
                branch.Code,
                branch.Name,
                branch.Address,
                branch.ManagerName,
                todaySalesByBranch.GetValueOrDefault(branch.Id) + todayInventoryByBranch.GetValueOrDefault(branch.Id),
                lowStockByBranch.GetValueOrDefault(branch.Id),
                branch.IsActive))
            .ToArray();
    }

    private async Task<IReadOnlyDictionary<Guid, int>> BuildLowStockLookupAsync(Guid? effectiveBranchId, CancellationToken cancellationToken)
    {
        var lowStockQuery =
            from inventory in dbContext.InventoryItems.AsNoTracking()
            join ingredient in dbContext.Ingredients.AsNoTracking() on inventory.IngredientId equals ingredient.Id
            where inventory.InStockQuantity <= ingredient.ReorderLevel
            select inventory;

        if (effectiveBranchId.HasValue)
        {
            lowStockQuery = lowStockQuery.Where(inventory => inventory.BranchId == effectiveBranchId.Value);
        }

        return await lowStockQuery
            .GroupBy(inventory => inventory.BranchId)
            .ToDictionaryAsync(group => group.Key, group => group.Count(), cancellationToken);
    }

    private async Task<IReadOnlyCollection<ProductSummaryDto>> BuildProductSummariesAsync(
        IReadOnlyCollection<Domain.Entities.SaleOrder> paidOrders,
        CancellationToken cancellationToken)
    {
        var products = await dbContext.Products.AsNoTracking().OrderBy(product => product.Name).ToListAsync(cancellationToken);
        var soldQuantityByProduct = paidOrders
            .SelectMany(order => order.Items)
            .GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));

        return products
            .Select(product => new ProductSummaryDto(
                product.Id,
                product.Sku,
                product.Name,
                product.Category,
                product.Price,
                product.ImageUrl,
                product.IsAvailable,
                soldQuantityByProduct.GetValueOrDefault(product.Id)))
            .Where(product => product.SoldQuantity > 0)
            .ToArray();
    }

    private Guid? ResolveBranchScope()
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to access dashboard data.");
        }

        if (currentUser.Role == UserRole.Administrator)
        {
            return null;
        }

        return currentUser.BranchId
            ?? throw new UnauthorizedAccessException("Your account is not assigned to a branch.");
    }
}
