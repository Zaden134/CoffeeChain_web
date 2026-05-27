using CoffeeChainManagement.Application.DTOs.Common;
using CoffeeChainManagement.Application.DTOs.Inventory;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresInventoryService xu ly CRUD ton kho va filter theo quyen chi nhanh.
internal sealed class PostgresInventoryService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser) : IInventoryService
{
    public async Task<IReadOnlyCollection<InventoryItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureReadable();

        var branchQuery = dbContext.Branches.AsNoTracking();
        var inventoryQuery = dbContext.InventoryItems.AsNoTracking();

        if (currentUser.Role == UserRole.BranchManager)
        {
            inventoryQuery = inventoryQuery.Where(item => item.BranchId == currentUser.BranchId);
            branchQuery = branchQuery.Where(branch => branch.Id == currentUser.BranchId);
        }

        var branches = await branchQuery.ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        var ingredients = await dbContext.Ingredients.AsNoTracking().ToDictionaryAsync(ingredient => ingredient.Id, cancellationToken);
        var items = await inventoryQuery.OrderBy(item => item.BranchId).ToListAsync(cancellationToken);

        return items.Select(item =>
        {
            var ingredient = ingredients[item.IngredientId];
            return new InventoryItemDto(
                item.Id,
                item.BranchId,
                branches.GetValueOrDefault(item.BranchId, "Khong ro chi nhanh"),
                item.IngredientId,
                ingredient.Name,
                ingredient.Unit,
                ingredient.ReorderLevel,
                item.InStockQuantity,
                item.ReservedQuantity,
                item.InStockQuantity <= ingredient.ReorderLevel);
        }).ToArray();
    }

    public async Task<InventoryItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureReadable();

        var item = await dbContext.InventoryItems.AsNoTracking().SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken);
        if (item is null || !CanAccessBranch(item.BranchId))
        {
            return null;
        }

        var ingredient = await dbContext.Ingredients.AsNoTracking().SingleAsync(entry => entry.Id == item.IngredientId, cancellationToken);
        var branch = await dbContext.Branches.AsNoTracking().SingleAsync(entry => entry.Id == item.BranchId, cancellationToken);

        return new InventoryItemDto(
            item.Id,
            item.BranchId,
            branch.Name,
            item.IngredientId,
            ingredient.Name,
            ingredient.Unit,
            ingredient.ReorderLevel,
            item.InStockQuantity,
            item.ReservedQuantity,
            item.InStockQuantity <= ingredient.ReorderLevel);
    }

    public async Task<InventoryItemDto> CreateAsync(UpsertInventoryItemRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable(request.BranchId);

        var ingredient = await ResolveIngredientAsync(request, cancellationToken);
        var item = new InventoryItem
        {
            BranchId = request.BranchId,
            IngredientId = ingredient.Id,
            InStockQuantity = request.InStockQuantity,
            ReservedQuantity = request.ReservedQuantity
        };

        dbContext.InventoryItems.Add(item);
        await dbContext.SaveChangesAsync(cancellationToken);
        return await MapInventoryItemAsync(item, cancellationToken);
    }

    public async Task<InventoryItemDto> UpdateAsync(Guid id, UpsertInventoryItemRequestDto request, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.InventoryItems.SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Inventory item not found.");

        EnsureWritable(item.BranchId);
        EnsureWritable(request.BranchId);

        var ingredient = await ResolveIngredientAsync(request, cancellationToken);

        item.BranchId = request.BranchId;
        item.IngredientId = ingredient.Id;
        item.InStockQuantity = request.InStockQuantity;
        item.ReservedQuantity = request.ReservedQuantity;
        item.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return await MapInventoryItemAsync(item, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.InventoryItems.SingleOrDefaultAsync(entry => entry.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Inventory item not found.");

        EnsureWritable(item.BranchId);
        dbContext.InventoryItems.Remove(item);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<LookupDto>> GetIngredientLookupsAsync(CancellationToken cancellationToken = default)
    {
        EnsureReadable();
        return await dbContext.Ingredients.AsNoTracking()
            .OrderBy(ingredient => ingredient.Name)
            .Select(ingredient => new LookupDto(ingredient.Id, $"{ingredient.Name} ({ingredient.Unit})"))
            .ToArrayAsync(cancellationToken);
    }

    private async Task<Ingredient> ResolveIngredientAsync(UpsertInventoryItemRequestDto request, CancellationToken cancellationToken)
    {
        if (request.IngredientId.HasValue)
        {
            var existingIngredient = await dbContext.Ingredients.SingleOrDefaultAsync(entry => entry.Id == request.IngredientId.Value, cancellationToken);
            return existingIngredient ?? throw new KeyNotFoundException("Ingredient not found.");
        }

        if (string.IsNullOrWhiteSpace(request.IngredientName) || string.IsNullOrWhiteSpace(request.Unit))
        {
            throw new InvalidOperationException("Ingredient name and unit are required when creating a new ingredient.");
        }

        var ingredient = new Ingredient
        {
            Name = request.IngredientName.Trim(),
            Unit = request.Unit.Trim(),
            ReorderLevel = request.ReorderLevel
        };

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ingredient;
    }

    private async Task<InventoryItemDto> MapInventoryItemAsync(InventoryItem item, CancellationToken cancellationToken)
    {
        var ingredient = await dbContext.Ingredients.AsNoTracking().SingleAsync(entry => entry.Id == item.IngredientId, cancellationToken);
        var branch = await dbContext.Branches.AsNoTracking().SingleAsync(entry => entry.Id == item.BranchId, cancellationToken);

        return new InventoryItemDto(
            item.Id,
            item.BranchId,
            branch.Name,
            item.IngredientId,
            ingredient.Name,
            ingredient.Unit,
            ingredient.ReorderLevel,
            item.InStockQuantity,
            item.ReservedQuantity,
            item.InStockQuantity <= ingredient.ReorderLevel);
    }

    private void EnsureReadable()
    {
        if (!currentUser.IsAuthenticated || (currentUser.Role != UserRole.Administrator && currentUser.Role != UserRole.BranchManager))
        {
            throw new UnauthorizedAccessException("You do not have permission to access inventory.");
        }
    }

    private void EnsureWritable(Guid branchId)
    {
        if (!currentUser.IsAuthenticated || (currentUser.Role != UserRole.Administrator && currentUser.Role != UserRole.BranchManager))
        {
            throw new UnauthorizedAccessException("You do not have permission to modify inventory.");
        }

        if (currentUser.Role == UserRole.BranchManager && currentUser.BranchId != branchId)
        {
            throw new UnauthorizedAccessException("Branch managers can only modify inventory in their own branch.");
        }
    }

    private bool CanAccessBranch(Guid branchId)
        => currentUser.Role == UserRole.Administrator || currentUser.BranchId == branchId;
}
