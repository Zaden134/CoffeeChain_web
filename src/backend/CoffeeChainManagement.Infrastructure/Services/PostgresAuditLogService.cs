using CoffeeChainManagement.Application.DTOs.Audit;
using CoffeeChainManagement.Application.DTOs.Common;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresAuditLogService ghi lai thao tac va actor hien tai vao audit_logs.
internal sealed class PostgresAuditLogService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser) : IAuditLogService
{
    public async Task<PagedResultDto<AuditLogDto>> GetPagedAsync(
        AuditLogQueryDto query,
        CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.Role != UserRole.Administrator)
        {
            throw new UnauthorizedAccessException("Only administrators can access audit logs.");
        }

        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 5, 100);
        var logsQuery = dbContext.AuditLogs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            logsQuery = logsQuery.Where(entry =>
                entry.Action.ToLower().Contains(search)
                || entry.EntityType.ToLower().Contains(search)
                || entry.Details.ToLower().Contains(search)
                || (entry.Username != null && entry.Username.ToLower().Contains(search))
                || (entry.EntityId != null && entry.EntityId.ToLower().Contains(search)));
        }

        var totalItems = await logsQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
        var items = await logsQuery
            .OrderByDescending(entry => entry.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(entry => new AuditLogDto(
                entry.Id,
                entry.CreatedAtUtc,
                entry.EmployeeId,
                entry.Username,
                entry.BranchId,
                entry.Action,
                entry.EntityType,
                entry.EntityId,
                entry.Details,
                entry.IsSuccess))
            .ToArrayAsync(cancellationToken);

        return new PagedResultDto<AuditLogDto>(items, page, pageSize, totalItems, totalPages);
    }

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
