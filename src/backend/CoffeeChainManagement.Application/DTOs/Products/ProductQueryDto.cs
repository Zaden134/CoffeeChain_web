namespace CoffeeChainManagement.Application.DTOs.Products;

public sealed record ProductQueryDto(
    int Page = 1,
    int PageSize = 12,
    string? Search = null,
    string? Category = null,
    bool? IsAvailable = null);
