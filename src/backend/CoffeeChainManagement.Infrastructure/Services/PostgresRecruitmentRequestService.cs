using CoffeeChainManagement.Application.DTOs.Recruitment;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresRecruitmentRequestService xu ly luong yeu cau tuyen dung va phe duyet.
internal sealed class PostgresRecruitmentRequestService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser,
    IAuditLogService auditLogService) : IRecruitmentRequestService
{
    public async Task<IReadOnlyCollection<RecruitmentRequestDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        EnsureReadable();

        var query = dbContext.RecruitmentRequests.AsNoTracking();
        if (currentUser.Role == UserRole.BranchManager)
        {
            query = query.Where(request => request.BranchId == currentUser.BranchId);
        }

        var requests = await query.OrderByDescending(request => request.CreatedAtUtc).ToListAsync(cancellationToken);
        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        var employees = await dbContext.Employees.AsNoTracking().ToDictionaryAsync(employee => employee.Id, employee => employee.FullName, cancellationToken);

        return requests.Select(request => MapRequest(request, branches, employees)).ToArray();
    }

    public async Task<RecruitmentRequestDto> CreateAsync(CreateRecruitmentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.Role != UserRole.BranchManager || currentUser.BranchId != request.BranchId)
        {
            throw new UnauthorizedAccessException("Only the branch manager of this branch can create a recruitment request.");
        }

        var entity = new RecruitmentRequest
        {
            BranchId = request.BranchId,
            RequestedByEmployeeId = currentUser.UserId,
            PositionTitle = request.PositionTitle.Trim(),
            Quantity = request.Quantity,
            Reason = request.Reason.Trim(),
            Status = RecruitmentRequestStatus.Pending
        };

        dbContext.RecruitmentRequests.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        var branch = await dbContext.Branches.AsNoTracking().SingleAsync(item => item.Id == entity.BranchId, cancellationToken);
        var employee = await dbContext.Employees.AsNoTracking().SingleAsync(item => item.Id == entity.RequestedByEmployeeId, cancellationToken);
        var result = MapRequest(entity, new Dictionary<Guid, string> { [branch.Id] = branch.Name }, new Dictionary<Guid, string> { [employee.Id] = employee.FullName });
        await auditLogService.WriteAsync("RECRUITMENT_CREATE", nameof(RecruitmentRequest), $"Requested {entity.PositionTitle} x{entity.Quantity}", true, employee.Id, employee.Username, branch.Id, entity.Id, cancellationToken);
        return result;
    }

    public async Task<RecruitmentRequestDto> ReviewAsync(Guid id, ReviewRecruitmentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated || currentUser.Role != UserRole.Administrator)
        {
            throw new UnauthorizedAccessException("Only administrators can review recruitment requests.");
        }

        var entity = await dbContext.RecruitmentRequests.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Recruitment request not found.");

        entity.Status = request.Decision == "Approved"
            ? RecruitmentRequestStatus.Approved
            : RecruitmentRequestStatus.Rejected;
        entity.AdminNote = string.IsNullOrWhiteSpace(request.AdminNote) ? null : request.AdminNote.Trim();
        entity.ReviewedAtUtc = DateTime.UtcNow;
        entity.ReviewedByEmployeeId = currentUser.UserId;
        entity.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        var branch = await dbContext.Branches.AsNoTracking().SingleAsync(item => item.Id == entity.BranchId, cancellationToken);
        var employee = await dbContext.Employees.AsNoTracking().SingleAsync(item => item.Id == entity.RequestedByEmployeeId, cancellationToken);
        var result = MapRequest(entity, new Dictionary<Guid, string> { [branch.Id] = branch.Name }, new Dictionary<Guid, string> { [employee.Id] = employee.FullName });
        await auditLogService.WriteAsync("RECRUITMENT_REVIEW", nameof(RecruitmentRequest), $"{request.Decision} recruitment request", true, currentUser.UserId, currentUser.Username, branch.Id, entity.Id, cancellationToken);
        return result;
    }

    private void EnsureReadable()
    {
        if (!currentUser.IsAuthenticated || (currentUser.Role != UserRole.Administrator && currentUser.Role != UserRole.BranchManager))
        {
            throw new UnauthorizedAccessException("You do not have permission to access recruitment requests.");
        }
    }

    private static RecruitmentRequestDto MapRequest(
        RecruitmentRequest request,
        IReadOnlyDictionary<Guid, string> branches,
        IReadOnlyDictionary<Guid, string> employees)
        => new(
            request.Id,
            request.BranchId,
            branches.GetValueOrDefault(request.BranchId, "Khong ro chi nhanh"),
            request.RequestedByEmployeeId,
            employees.GetValueOrDefault(request.RequestedByEmployeeId, "Khong ro nhan vien"),
            request.PositionTitle,
            request.Quantity,
            request.Reason,
            request.Status.ToString(),
            request.AdminNote,
            request.CreatedAtUtc,
            request.ReviewedAtUtc);
}
