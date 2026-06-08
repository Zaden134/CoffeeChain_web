namespace CoffeeChainManagement.Application.DTOs.Auth;

// UserProfileDto la payload gon cho user hien tai tren frontend.
public sealed record UserProfileDto(
    Guid Id,
    string Username,
    string FullName,
    string Email,
    string Role,
    Guid? BranchId);
