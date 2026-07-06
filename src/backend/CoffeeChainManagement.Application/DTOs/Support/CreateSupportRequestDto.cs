using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Support;

public sealed class CreateSupportRequestDto
{
    [Required]
    [StringLength(120)]
    public string Subject { get; init; } = string.Empty;

    [Required]
    [StringLength(1000)]
    public string Message { get; init; } = string.Empty;
}
