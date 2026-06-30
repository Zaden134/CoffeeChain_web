namespace CoffeeChainManagement.Application.DTOs.Products;

// UpsertProductRequestDto nhan du lieu tu form tao/sua san pham.
public sealed record UpsertProductRequestDto(
    string Sku,
    string Name,
    string Category,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable);
