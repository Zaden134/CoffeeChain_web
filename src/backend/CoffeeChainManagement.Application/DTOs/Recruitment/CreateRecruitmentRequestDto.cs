using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Recruitment;

// CreateRecruitmentRequestDto nhan de xuat tuyen dung moi tu quan ly chi nhanh.
public sealed class CreateRecruitmentRequestDto
{
    [Required]
    public Guid BranchId { get; init; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string PositionTitle { get; init; } = string.Empty;

    [Range(1, 100)]
    public int Quantity { get; init; }

    [Required]
    [StringLength(1000, MinimumLength = 10)]
    public string Reason { get; init; } = string.Empty;
}
