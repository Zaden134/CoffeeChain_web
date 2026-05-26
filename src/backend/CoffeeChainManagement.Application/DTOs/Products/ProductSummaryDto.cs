namespace CoffeeChainManagement.Application.DTOs.Products;

// ProductSummaryDto dung cho menu quan tri va widget mon ban chay.
public sealed record ProductSummaryDto(
    Guid Id,
    string Sku,
    string Name,
    string Category,
    decimal Price,
    string ImageUrl,
    bool IsAvailable,
    int SoldQuantity);
