using System.ComponentModel.DataAnnotations;

namespace CoffeeChainManagement.Application.DTOs.Employees;

// UpsertEmployeeRequestDto nhan du lieu tao/sua nhan vien tu frontend.
public sealed class UpsertEmployeeRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string FullName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; init; } = string.Empty;

    [Required]
    public string Role { get; init; } = string.Empty;

    public Guid? BranchId { get; init; }

    [MinLength(8)]
    public string? Password { get; init; }

    public bool IsActive { get; init; } = true;
}
