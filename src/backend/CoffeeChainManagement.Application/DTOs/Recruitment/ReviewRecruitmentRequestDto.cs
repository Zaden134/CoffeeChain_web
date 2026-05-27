using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Recruitment;

// ReviewRecruitmentRequestDto nhan thao tac phe duyet hoac tu choi tu admin.
public sealed class ReviewRecruitmentRequestDto
{
    [Required]
    [RegularExpression("Approved|Rejected")]
    public string Decision { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? AdminNote { get; init; }
}
