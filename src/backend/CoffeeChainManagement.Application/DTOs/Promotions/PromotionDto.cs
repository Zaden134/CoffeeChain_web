namespace CoffeeChainManagement.Application.DTOs.Promotions;

// PromotionDto la view model khuyen mai cho danh sach va form frontend.
public sealed record PromotionDto(
    Guid Id,
    string Code,
    string Name,
    decimal? DiscountPercent,
    decimal? DiscountAmount,
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BranchId,
    string? BranchName,
    string? CustomerSegment,
    string? CustomerPhone,
    bool IsActive);
