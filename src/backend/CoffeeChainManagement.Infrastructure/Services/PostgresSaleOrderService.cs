using CoffeeChainManagement.Application.DTOs.Sales;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

public sealed class PostgresSaleOrderService(CoffeeChainDbContext dbContext) : ISaleOrderService
{
    public async Task<int> SyncOrdersAsync(SyncOrdersRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Orders == null || request.Orders.Count == 0)
        {
            return 0;
        }

        int syncedCount = 0;

        foreach (var orderDto in request.Orders)
        {
            // Check if order already exists (idempotent)
            bool exists = await dbContext.Set<SaleOrder>().AnyAsync(o => o.Id == orderDto.Id, cancellationToken);
            if (exists)
            {
                continue;
            }

            var saleOrder = new SaleOrder
            {
                Id = orderDto.Id,
                BranchId = orderDto.BranchId,
                EmployeeId = orderDto.EmployeeId,
                PaymentMethod = Enum.TryParse<PaymentMethod>(orderDto.PaymentMethod, true, out var pm) ? pm : PaymentMethod.Cash,
                Status = Enum.TryParse<OrderStatus>(orderDto.Status, true, out var status) ? status : OrderStatus.Paid,
                PromotionCode = orderDto.PromotionCode,
                DiscountAmount = orderDto.DiscountAmount,
                CreatedAtUtc = orderDto.CreatedAtUtc,
                Items = orderDto.Items.Select(i => new SaleOrderItem
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            await dbContext.Set<SaleOrder>().AddAsync(saleOrder, cancellationToken);

            // Calculate total ingredients required for this order
            var productIds = orderDto.Items.Select(i => i.ProductId).Distinct().ToList();
            var recipes = await dbContext.Recipes.Include(r => r.Ingredients)
                                                 .ThenInclude(ri => ri.Ingredient)
                                                 .Where(r => productIds.Contains(r.ProductId)).ToListAsync(cancellationToken);

            var requiredIngredients = new Dictionary<Guid, (decimal Quantity, string Name)>();

            foreach (var item in orderDto.Items)
            {
                var recipe = recipes.FirstOrDefault(r => r.ProductId == item.ProductId);
                if (recipe != null)
                {
                    foreach (var ing in recipe.Ingredients)
                    {
                        if (requiredIngredients.ContainsKey(ing.IngredientId))
                        {
                            var existing = requiredIngredients[ing.IngredientId];
                            requiredIngredients[ing.IngredientId] = (existing.Quantity + (ing.RequiredQuantity * item.Quantity), existing.Name);
                        }
                        else
                        {
                            requiredIngredients[ing.IngredientId] = (ing.RequiredQuantity * item.Quantity, ing.Ingredient?.Name ?? "Unknown");
                        }
                    }
                }
            }

            // Validate inventory
            var branchInventory = await dbContext.InventoryItems
                .Where(inv => inv.BranchId == orderDto.BranchId && requiredIngredients.Keys.Contains(inv.IngredientId))
                .ToListAsync(cancellationToken);

            foreach (var req in requiredIngredients)
            {
                var inventoryItem = branchInventory.FirstOrDefault(inv => inv.IngredientId == req.Key);
                if (inventoryItem == null)
                {
                    throw new InvalidOperationException($"Chi nhánh không có nguyên liệu '{req.Value.Name}' trong kho để pha chế.");
                }

                if (inventoryItem.InStockQuantity - inventoryItem.ReservedQuantity < req.Value.Quantity)
                {
                    throw new InvalidOperationException($"Kho chi nhánh không đủ số lượng nguyên liệu '{req.Value.Name}' (Cần: {req.Value.Quantity}, Có thể dùng: {inventoryItem.InStockQuantity - inventoryItem.ReservedQuantity}).");
                }
                
                // Deduct from stock
                inventoryItem.InStockQuantity -= req.Value.Quantity;
            }

            syncedCount++;
        }

        if (syncedCount > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return syncedCount;
    }
}
