using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// RefreshTokenSession luu trang thai refresh token de ho tro xoay vong va revoke.
public sealed class RefreshTokenSession : BaseEntity
{
    public required Guid EmployeeId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiresAtUtc { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
}
