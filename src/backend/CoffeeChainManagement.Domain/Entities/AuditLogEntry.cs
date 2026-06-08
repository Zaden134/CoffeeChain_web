using CoffeeChainManagement.Domain.Common;

namespace CoffeeChainManagement.Domain.Entities;

// AuditLogEntry ghi lai thao tac quan trong de trace va phuc vu kiem tra.
public sealed class AuditLogEntry : BaseEntity
{
    public Guid? EmployeeId { get; set; }
    public string? Username { get; set; }
    public required string Action { get; set; }
    public required string EntityType { get; set; }
    public string? EntityId { get; set; }
    public required string Details { get; set; }
    public Guid? BranchId { get; set; }
    public required bool IsSuccess { get; set; }
}
