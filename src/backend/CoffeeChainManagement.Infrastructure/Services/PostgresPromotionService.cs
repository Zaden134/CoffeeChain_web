using CoffeeChainManagement.Application.DTOs.Promotions;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Services;

// PostgresPromotionService xu ly CRUD khuyen mai va quyen theo action.
internal sealed class PostgresPromotionService(
    CoffeeChainDbContext dbContext,
    ICurrentUserContext currentUser,
    IAuditLogService auditLogService) : IPromotionService
{
    public async Task<IReadOnlyCollection<PromotionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to access promotions.");
        }

        var query = dbContext.Promotions.AsNoTracking();
        if (currentUser.Role is UserRole.BranchManager or UserRole.Cashier)
        {
            query = query.Where(promotion => promotion.BranchId == null || promotion.BranchId == currentUser.BranchId);
        }

        if (currentUser.Role == UserRole.Cashier)
        {
            query = query.Where(promotion => promotion.IsActive);
        }

        var promotions = await query.OrderByDescending(promotion => promotion.StartDate).ToListAsync(cancellationToken);
        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        return promotions.Select(promotion => MapPromotion(promotion, branches)).ToArray();
    }

    public async Task<PromotionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to access promotions.");
        }

        var promotion = await dbContext.Promotions.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (promotion is null || (currentUser.Role == UserRole.Cashier && !promotion.IsActive))
        {
            return null;
        }

        if (currentUser.Role is UserRole.BranchManager or UserRole.Cashier
            && promotion.BranchId.HasValue
            && promotion.BranchId != currentUser.BranchId)
        {
            return null;
        }

        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        return MapPromotion(promotion, branches);
    }

    public async Task<PromotionDto> CreateAsync(UpsertPromotionRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable();
        EnsureDateRange(request.StartDate, request.EndDate);
        EnsureBranchScope(request.BranchId);

        var promotion = new Promotion
        {
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
            DiscountPercent = request.DiscountPercent,
            DiscountAmount = request.DiscountAmount,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            BranchId = request.BranchId,
            CustomerSegment = string.IsNullOrWhiteSpace(request.CustomerSegment) ? null : request.CustomerSegment.Trim(),
            CustomerPhone = string.IsNullOrWhiteSpace(request.CustomerPhone) ? null : request.CustomerPhone.Trim(),
            IsActive = request.IsActive
        };

        dbContext.Promotions.Add(promotion);
        await dbContext.SaveChangesAsync(cancellationToken);
        var result = await MapPromotionAsync(promotion, cancellationToken);
        await auditLogService.WriteAsync("PROMOTION_CREATE", nameof(Promotion), $"Created promotion {promotion.Name}", true, promotion.Id, entityId: promotion.Id, cancellationToken: cancellationToken);
        return result;
    }

    public async Task<PromotionDto> UpdateAsync(Guid id, UpsertPromotionRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable();
        EnsureDateRange(request.StartDate, request.EndDate);
        EnsureBranchScope(request.BranchId);

        var promotion = await dbContext.Promotions.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Promotion not found.");
        EnsureBranchScope(promotion.BranchId);

        promotion.Code = request.Code.Trim();
        promotion.Name = request.Name.Trim();
        promotion.DiscountPercent = request.DiscountPercent;
        promotion.DiscountAmount = request.DiscountAmount;
        promotion.StartDate = request.StartDate;
        promotion.EndDate = request.EndDate;
        promotion.BranchId = request.BranchId;
        promotion.CustomerSegment = string.IsNullOrWhiteSpace(request.CustomerSegment) ? null : request.CustomerSegment.Trim();
        promotion.CustomerPhone = string.IsNullOrWhiteSpace(request.CustomerPhone) ? null : request.CustomerPhone.Trim();
        promotion.IsActive = request.IsActive;
        promotion.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        var result = await MapPromotionAsync(promotion, cancellationToken);
        await auditLogService.WriteAsync("PROMOTION_UPDATE", nameof(Promotion), $"Updated promotion {promotion.Name}", true, promotion.Id, entityId: promotion.Id, cancellationToken: cancellationToken);
        return result;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureWritable();

        var promotion = await dbContext.Promotions.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Promotion not found.");
        EnsureBranchScope(promotion.BranchId);

        dbContext.Promotions.Remove(promotion);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.WriteAsync("PROMOTION_DELETE", nameof(Promotion), $"Deleted promotion {promotion.Name}", true, promotion.Id, entityId: promotion.Id, cancellationToken: cancellationToken);
    }

    private void EnsureWritable()
    {
        if (!currentUser.IsAuthenticated || (currentUser.Role != UserRole.Administrator && currentUser.Role != UserRole.BranchManager))
        {
            throw new UnauthorizedAccessException("You do not have permission to modify promotions.");
        }
    }

    private static void EnsureDateRange(DateOnly startDate, DateOnly endDate)
    {
        if (endDate < startDate)
        {
            throw new InvalidOperationException("End date must be greater than or equal to start date.");
        }
    }

    private void EnsureBranchScope(Guid? branchId)
    {
        if (currentUser.Role == UserRole.Administrator)
        {
            return;
        }

        if (branchId != currentUser.BranchId)
        {
            throw new UnauthorizedAccessException("Branch managers can only manage promotions for their own branch.");
        }
    }

    private async Task<PromotionDto> MapPromotionAsync(Promotion promotion, CancellationToken cancellationToken)
    {
        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(branch => branch.Id, branch => branch.Name, cancellationToken);
        return MapPromotion(promotion, branches);
    }

    private static PromotionDto MapPromotion(Promotion promotion, IReadOnlyDictionary<Guid, string> branches)
        => new(
            promotion.Id,
            promotion.Code,
            promotion.Name,
            promotion.DiscountPercent,
            promotion.DiscountAmount,
            promotion.StartDate,
            promotion.EndDate,
            promotion.BranchId,
            promotion.BranchId.HasValue && branches.TryGetValue(promotion.BranchId.Value, out var branchName) ? branchName : null,
            promotion.CustomerSegment,
            promotion.CustomerPhone,
            promotion.IsActive);
}
