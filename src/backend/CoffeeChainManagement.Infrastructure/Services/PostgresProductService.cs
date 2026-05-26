using CoffeeChainManagement.Application.DTOs.Products;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresProductService doc menu va so luong ban ra tu du lieu don hang that.
internal sealed class PostgresProductService(CoffeeChainDbContext dbContext) : IProductService
{
    public async Task<IReadOnlyCollection<ProductSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await dbContext.Products.AsNoTracking().OrderBy(product => product.Name).ToListAsync(cancellationToken);
        var paidOrders = await dbContext.SaleOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.Status == OrderStatus.Paid)
            .ToListAsync(cancellationToken);

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
            .ToArray();
    }
}
