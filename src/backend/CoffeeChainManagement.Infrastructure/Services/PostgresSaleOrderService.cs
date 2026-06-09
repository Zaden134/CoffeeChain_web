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
            syncedCount++;
        }

        if (syncedCount > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return syncedCount;
    }
}
