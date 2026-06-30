using CoffeeChainManagement.Application.DTOs.Audit;
using CoffeeChainManagement.Application.DTOs.Common;

namespace CoffeeChainManagement.Application.Interfaces;

// IAuditLogService ghi lai thao tac quan trong va ket qua xu ly.
public interface IAuditLogService
{
    Task<PagedResultDto<AuditLogDto>> GetPagedAsync(
        AuditLogQueryDto query,
        CancellationToken cancellationToken = default);

    Task WriteAsync(
        string action,
        string entityType,
        string details,
        bool isSuccess,
        Guid? employeeId = null,
        string? username = null,
        Guid? branchId = null,
        Guid? entityId = null,
        CancellationToken cancellationToken = default);
}
