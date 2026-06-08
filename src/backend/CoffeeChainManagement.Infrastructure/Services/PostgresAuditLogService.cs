using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Infrastructure.Persistence;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresAuditLogService ghi lai thao tac va actor hien tai vao audit_logs.
internal sealed class PostgresAuditLogService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser) : IAuditLogService
{
    public async Task WriteAsync(
        string action,
        string entityType,
        string details,
        bool isSuccess,
        Guid? employeeId = null,
        string? username = null,
        Guid? branchId = null,
        Guid? entityId = null,
        CancellationToken cancellationToken = default)
    {
        var entry = new AuditLogEntry
        {
            EmployeeId = employeeId ?? (currentUser.IsAuthenticated ? currentUser.UserId : null),
            Username = username ?? currentUser.Username,
            BranchId = branchId ?? (currentUser.IsAuthenticated ? currentUser.BranchId : null),
            Action = action,
            EntityType = entityType,
            EntityId = entityId?.ToString(),
            Details = details,
            IsSuccess = isSuccess
        };

        dbContext.AuditLogs.Add(entry);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
