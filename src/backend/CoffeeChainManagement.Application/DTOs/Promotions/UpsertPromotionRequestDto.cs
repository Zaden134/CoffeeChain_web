using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Promotions;

// UpsertPromotionRequestDto nhan du lieu tao/sua khuyen mai.
public sealed class UpsertPromotionRequestDto
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; init; } = string.Empty;

    [Range(0, 100)]
    public decimal DiscountPercent { get; init; }

    [Required]
    public DateOnly StartDate { get; init; }

    [Required]
    public DateOnly EndDate { get; init; }

    public bool IsActive { get; init; } = true;
}
