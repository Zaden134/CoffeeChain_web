namespace CoffeeChainManagement.Application.DTOs.Promotions;

// PromotionDto la view model khuyen mai cho danh sach va form frontend.
public sealed record PromotionDto(
    Guid Id,
    string Name,
    decimal DiscountPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsActive);
