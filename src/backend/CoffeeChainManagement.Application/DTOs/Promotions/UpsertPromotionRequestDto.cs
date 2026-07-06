using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Promotions;

// UpsertPromotionRequestDto nhan du lieu tao/sua khuyen mai.
public sealed class UpsertPromotionRequestDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; init; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; init; } = string.Empty;

    [Range(0, 100)]
    public decimal? DiscountPercent { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? DiscountAmount { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    public Guid? BranchId { get; init; }

    [StringLength(100)]
    public string? CustomerSegment { get; init; }

    [StringLength(30)]
    public string? CustomerPhone { get; init; }

    public bool IsActive { get; init; } = true;
}
