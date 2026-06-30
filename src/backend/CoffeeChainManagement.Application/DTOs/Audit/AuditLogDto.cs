namespace CoffeeChainManagement.Application.DTOs.Audit;

// AuditLogDto dung cho man hinh admin theo doi lich su thao tac.
public sealed record AuditLogDto(
    Guid Id,
    DateTime CreatedAtUtc,
    Guid? EmployeeId,
    string? Username,
    Guid? BranchId,
    string Action,
    string EntityType,
    string? EntityId,
    string Details,
    bool IsSuccess);
