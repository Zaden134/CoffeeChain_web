namespace CoffeeChainManagement.Application.DTOs.Auth;

public sealed record UserSessionDto(
    Guid Id,
    Guid EmployeeId,
    string Username,
    string FullName,
    string Role,
    Guid? BranchId,
    DateTime CreatedAtUtc,
    DateTime ExpiresAtUtc,
    DateTime? RevokedAtUtc,
    string? CreatedByIp,
    string? RevokedByIp,
    bool IsActive);
