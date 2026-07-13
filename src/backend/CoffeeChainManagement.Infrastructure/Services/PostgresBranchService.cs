using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresBranchService doc du lieu chi nhanh va tong hop so lieu tu PostgreSQL.
internal sealed class PostgresBranchService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser,
    IAuditLogService auditLogService) : IBranchService
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

    public async Task<BranchSummaryDto?> UpdateAsync(Guid id, UpsertBranchRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable(id);

        var branch = await dbContext.Branches.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (branch is null)
        {
            return null;
        }

        if (currentUser.Role == UserRole.BranchManager && request.IsActive != branch.IsActive)
        {
            throw new InvalidOperationException("Branch managers cannot change branch active status.");
        }

        var duplicateCode = await dbContext.Branches.AnyAsync(
            item => item.Id != id && item.Code.ToLower() == request.Code.Trim().ToLower(),
            cancellationToken);
        if (duplicateCode)
        {
            throw new InvalidOperationException("Branch code already exists.");
        }

        branch.Code = request.Code.Trim();
        branch.Name = request.Name.Trim();
        branch.Address = request.Address.Trim();
        branch.ManagerName = request.ManagerName.Trim();
        branch.IsActive = request.IsActive;
        branch.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(
            "BRANCH_UPDATE",
            nameof(Branch),
            $"Updated branch {branch.Code}",
            true,
            currentUser.UserId,
            currentUser.Username,
            branch.Id,
            branch.Id,
            cancellationToken);

        return await BuildBranchSummaryAsync(branch.Id, cancellationToken);
    }

    private void EnsureWritable(Guid branchId)
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to update branches.");
        }

        if (currentUser.Role == UserRole.Administrator)
        {
            return;
        }

        if (currentUser.Role == UserRole.BranchManager && currentUser.BranchId == branchId)
        {
            return;
        }

        throw new UnauthorizedAccessException("Branch managers can only update their own branch.");
    }

    private async Task<BranchSummaryDto?> BuildBranchSummaryAsync(Guid branchId, CancellationToken cancellationToken)
    {
        var summaries = await GetAllAsync(cancellationToken);
        return summaries.FirstOrDefault(branch => branch.Id == branchId);
    }
}
