using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoffeeChainManagement.Application.DTOs.Inventory;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

internal sealed class PostgresInventoryTransactionService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser,
    IAuditLogService auditLogService) : IInventoryTransactionService
{
    public async Task<List<InventoryTransactionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureReadable();

        IQueryable<InventoryTransaction> query = dbContext.InventoryTransactions
            .AsNoTracking()
            .Include(t => t.Ingredient)
            .Include(t => t.Branch);

        if (currentUser.Role is UserRole.BranchManager or UserRole.WarehouseStaff)
        {
            query = query.Where(t => t.BranchId == currentUser.BranchId);
        }

        var txs = await query
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return txs.Select(t => new InventoryTransactionDto
        {
            Id = t.Id,
            IngredientId = t.IngredientId,
            IngredientName = t.Ingredient?.Name ?? string.Empty,
            BranchId = t.BranchId,
            BranchName = t.Branch?.Name ?? string.Empty,
            Type = t.Type,
            Quantity = t.Quantity,
            ReferenceNumber = t.ReferenceNumber,
            Notes = t.Notes,
            CreatedBy = t.CreatedBy,
            CreatedAtUtc = t.CreatedAtUtc
        }).ToList();
    }

    public async Task<InventoryTransactionDto> CreateTransactionAsync(CreateInventoryTransactionRequestDto request, Guid employeeId, CancellationToken cancellationToken = default)
    {
        EnsureWritable(request.BranchId);

        var ingredient = await dbContext.Ingredients.FindAsync([request.IngredientId], cancellationToken)
            ?? throw new KeyNotFoundException("Ingredient not found.");

        var branch = await dbContext.Branches.FindAsync([request.BranchId], cancellationToken)
            ?? throw new KeyNotFoundException("Branch not found.");

        var tx = new InventoryTransaction
        {
            IngredientId = request.IngredientId,
            BranchId = request.BranchId,
            Type = request.Type,
            Quantity = request.Type == TransactionType.Export ? -request.Quantity : request.Quantity,
            ReferenceNumber = request.ReferenceNumber,
            Notes = request.Notes,
            CreatedBy = employeeId
        };

        dbContext.InventoryTransactions.Add(tx);

        var inventoryItem = await dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.IngredientId == request.IngredientId && i.BranchId == request.BranchId, cancellationToken);

        if (inventoryItem == null)
        {
            inventoryItem = new InventoryItem
            {
                IngredientId = request.IngredientId,
                BranchId = request.BranchId,
                InStockQuantity = tx.Quantity,
                ReservedQuantity = 0
            };
            dbContext.InventoryItems.Add(inventoryItem);
        }
        else
        {
            inventoryItem.InStockQuantity += tx.Quantity;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync(
            "INVENTORY_TRANSACTION_CREATE",
            nameof(InventoryTransaction),
            $"Created inventory transaction {tx.ReferenceNumber}",
            true,
            employeeId,
            branchId: tx.BranchId,
            entityId: tx.Id,
            cancellationToken: cancellationToken);

        return new InventoryTransactionDto
        {
            Id = tx.Id,
            IngredientId = tx.IngredientId,
            IngredientName = ingredient.Name,
            BranchId = tx.BranchId,
            BranchName = branch.Name,
            Type = tx.Type,
            Quantity = tx.Quantity,
            ReferenceNumber = tx.ReferenceNumber,
            Notes = tx.Notes,
            CreatedBy = tx.CreatedBy,
            CreatedAtUtc = tx.CreatedAtUtc
        };
    }

    private void EnsureReadable()
    {
        if (!currentUser.IsAuthenticated || currentUser.Role is not (UserRole.Administrator or UserRole.BranchManager or UserRole.WarehouseStaff))
        {
            throw new UnauthorizedAccessException("You do not have permission to access inventory transactions.");
        }
    }

    private void EnsureWritable(Guid branchId)
    {
        EnsureReadable();

        if (currentUser.Role is (UserRole.BranchManager or UserRole.WarehouseStaff) && currentUser.BranchId != branchId)
        {
            throw new UnauthorizedAccessException("You can only create inventory transactions in your own branch.");
        }
    }
}
