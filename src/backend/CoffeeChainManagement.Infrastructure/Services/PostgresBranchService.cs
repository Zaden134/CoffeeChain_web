using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresBranchService doc du lieu chi nhanh va tong hop so lieu tu PostgreSQL.
internal sealed class PostgresBranchService(CoffeeChainDbContext dbContext) : IBranchService
{
    public async Task<IReadOnlyCollection<BranchSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var startOfDayUtc = DateTime.UtcNow.Date;

        var revenueByBranch = await dbContext.SaleOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.Status == OrderStatus.Paid && order.CreatedAtUtc >= startOfDayUtc)
            .ToListAsync(cancellationToken);

        var revenueLookup = revenueByBranch
            .GroupBy(order => order.BranchId)
            .ToDictionary(
                group => group.Key,
                group => group.Sum(order => order.Items.Sum(item => item.LineTotal)));
        var inventoryExpenseLookup = await dbContext.InventoryTransactions
            .AsNoTracking()
            .Where(transaction => transaction.CreatedAtUtc >= startOfDayUtc)
            .GroupBy(transaction => transaction.BranchId)
            .ToDictionaryAsync(
                group => group.Key,
                group => group.Sum(transaction => transaction.TransactionAmount),
                cancellationToken);

        var lowStockByBranch = await (
            from inventory in dbContext.InventoryItems.AsNoTracking()
            join ingredient in dbContext.Ingredients.AsNoTracking() on inventory.IngredientId equals ingredient.Id
            where inventory.InStockQuantity <= ingredient.ReorderLevel
            group inventory by inventory.BranchId
            into grouped
            select new
            {
                BranchId = grouped.Key,
                Count = grouped.Count()
            })
            .ToDictionaryAsync(item => item.BranchId, item => item.Count, cancellationToken);

        var branches = await dbContext.Branches
            .AsNoTracking()
            .OrderBy(branch => branch.Name)
            .ToListAsync(cancellationToken);

        return branches
            .Select(branch => new BranchSummaryDto(
                branch.Id,
                branch.Code,
                branch.Name,
                branch.Address,
                branch.ManagerName,
                revenueLookup.GetValueOrDefault(branch.Id) + inventoryExpenseLookup.GetValueOrDefault(branch.Id),
                lowStockByBranch.GetValueOrDefault(branch.Id),
                branch.IsActive))
            .ToArray();
    }
}
