namespace CoffeeChainManagement.Application.DTOs.Audit;

// AuditLogQueryDto gom filter co ban cho audit log.
public sealed record AuditLogQueryDto(
    int Page = 1,
    int PageSize = 20,
    string? Search = null);
