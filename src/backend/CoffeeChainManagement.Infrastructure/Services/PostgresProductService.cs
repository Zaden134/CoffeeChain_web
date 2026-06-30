using CoffeeChainManagement.Application.DTOs.Common;
using CoffeeChainManagement.Application.DTOs.Products;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresProductService doc menu va so luong ban ra tu du lieu don hang that.
internal sealed class PostgresProductService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser,
    IAuditLogService auditLogService) : IProductService
{
    public async Task<IReadOnlyCollection<ProductSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureReadable();
        var products = await dbContext.Products.AsNoTracking().OrderBy(product => product.Name).ToListAsync(cancellationToken);
        var soldQuantityByProduct = await GetSoldQuantityByProductAsync(cancellationToken);

        return products.Select(product => MapProduct(product, soldQuantityByProduct)).ToArray();
    }

    public async Task<PagedResultDto<ProductSummaryDto>> GetPagedAsync(ProductQueryDto query, CancellationToken cancellationToken = default)
    {
        EnsureReadable();

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 4, 100);
        var productsQuery = dbContext.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            productsQuery = productsQuery.Where(product =>
                product.Sku.ToLower().Contains(search)
                || product.Name.ToLower().Contains(search)
                || product.Category.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            var category = query.Category.Trim().ToLower();
            productsQuery = productsQuery.Where(product => product.Category.ToLower() == category);
        }

        if (query.IsAvailable.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.IsAvailable == query.IsAvailable.Value);
        }

        var totalItems = await productsQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        var products = await productsQuery
            .OrderBy(product => product.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        var soldQuantityByProduct = await GetSoldQuantityByProductAsync(cancellationToken);

        return new PagedResultDto<ProductSummaryDto>(
            products.Select(product => MapProduct(product, soldQuantityByProduct)).ToArray(),
            page,
            pageSize,
            totalItems,
            totalPages);
    }

    public async Task<ProductSummaryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureReadable();

        var product = await dbContext.Products.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var soldQuantityByProduct = await GetSoldQuantityByProductAsync(cancellationToken);
        return MapProduct(product, soldQuantityByProduct);
    }

    public async Task<ProductSummaryDto> CreateAsync(UpsertProductRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable();
        Validate(request);

        var sku = request.Sku.Trim();
        if (await dbContext.Products.AnyAsync(product => product.Sku == sku, cancellationToken))
        {
            throw new InvalidOperationException("SKU already exists.");
        }

        var product = new Product
        {
            Sku = sku,
            Name = request.Name.Trim(),
            Category = request.Category.Trim(),
            Price = request.Price,
            ImageUrl = request.ImageUrl?.Trim() ?? string.Empty,
            IsAvailable = request.IsAvailable
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync("PRODUCT_CREATE", nameof(Product), $"Created product {product.Sku}", true, entityId: product.Id, cancellationToken: cancellationToken);

        return MapProduct(product, new Dictionary<Guid, int>());
    }

    public async Task<ProductSummaryDto> UpdateAsync(Guid id, UpsertProductRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable();
        Validate(request);

        var product = await dbContext.Products.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        var sku = request.Sku.Trim();
        if (await dbContext.Products.AnyAsync(item => item.Id != id && item.Sku == sku, cancellationToken))
        {
            throw new InvalidOperationException("SKU already exists.");
        }

        product.Sku = sku;
        product.Name = request.Name.Trim();
        product.Category = request.Category.Trim();
        product.Price = request.Price;
        product.ImageUrl = request.ImageUrl?.Trim() ?? string.Empty;
        product.IsAvailable = request.IsAvailable;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync("PRODUCT_UPDATE", nameof(Product), $"Updated product {product.Sku}", true, entityId: product.Id, cancellationToken: cancellationToken);

        var soldQuantityByProduct = await GetSoldQuantityByProductAsync(cancellationToken);
        return MapProduct(product, soldQuantityByProduct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureWritable();

        var product = await dbContext.Products.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        product.IsAvailable = false;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync("PRODUCT_DELETE", nameof(Product), $"Deactivated product {product.Sku}", true, entityId: product.Id, cancellationToken: cancellationToken);
    }

    private async Task<Dictionary<Guid, int>> GetSoldQuantityByProductAsync(CancellationToken cancellationToken)
    {
        var paidOrders = await dbContext.SaleOrders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.Status == OrderStatus.Paid)
            .ToListAsync(cancellationToken);

        var soldQuantityByProduct = paidOrders
            .SelectMany(order => order.Items)
            .GroupBy(item => item.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));

        return soldQuantityByProduct;
    }

    private void EnsureReadable()
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to access products.");
        }
    }

    private void EnsureWritable()
    {
        if (!currentUser.IsAuthenticated || currentUser.Role is not (UserRole.Administrator or UserRole.BranchManager))
        {
            throw new UnauthorizedAccessException("You do not have permission to modify products.");
        }
    }

    private static void Validate(UpsertProductRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Category))
        {
            throw new InvalidOperationException("SKU, name and category are required.");
        }

        if (request.Price <= 0)
        {
            throw new InvalidOperationException("Price must be greater than zero.");
        }
    }

    private static ProductSummaryDto MapProduct(Product product, IReadOnlyDictionary<Guid, int> soldQuantityByProduct)
        => new(
            product.Id,
            product.Sku,
            product.Name,
            product.Category,
            product.Price,
            product.ImageUrl,
            product.IsAvailable,
            soldQuantityByProduct.GetValueOrDefault(product.Id));
}
