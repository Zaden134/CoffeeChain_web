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
    ICurrentUserContext currentUser) : IPromotionService
{
    public async Task<IReadOnlyCollection<PromotionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (!currentUser.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("You do not have permission to access promotions.");
        }

        var query = dbContext.Promotions.AsNoTracking();
        if (currentUser.Role == UserRole.Cashier)
        {
            query = query.Where(promotion => promotion.IsActive);
        }

        var promotions = await query.OrderByDescending(promotion => promotion.StartDate).ToListAsync(cancellationToken);
        return promotions.Select(MapPromotion).ToArray();
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

        return MapPromotion(promotion);
    }

    public async Task<PromotionDto> CreateAsync(UpsertPromotionRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable();
        EnsureDateRange(request.StartDate, request.EndDate);

        var promotion = new Promotion
        {
            Name = request.Name.Trim(),
            DiscountPercent = request.DiscountPercent,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = request.IsActive
        };

        dbContext.Promotions.Add(promotion);
        await dbContext.SaveChangesAsync(cancellationToken);
        return MapPromotion(promotion);
    }

    public async Task<PromotionDto> UpdateAsync(Guid id, UpsertPromotionRequestDto request, CancellationToken cancellationToken = default)
    {
        EnsureWritable();
        EnsureDateRange(request.StartDate, request.EndDate);

        var promotion = await dbContext.Promotions.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Promotion not found.");

        promotion.Name = request.Name.Trim();
        promotion.DiscountPercent = request.DiscountPercent;
        promotion.StartDate = request.StartDate;
        promotion.EndDate = request.EndDate;
        promotion.IsActive = request.IsActive;
        promotion.UpdatedAtUtc = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);
        return MapPromotion(promotion);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        EnsureWritable();

        var promotion = await dbContext.Promotions.SingleOrDefaultAsync(item => item.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Promotion not found.");

        dbContext.Promotions.Remove(promotion);
        await dbContext.SaveChangesAsync(cancellationToken);
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

    private static PromotionDto MapPromotion(Promotion promotion)
        => new(
            promotion.Id,
            promotion.Name,
            promotion.DiscountPercent,
            promotion.StartDate,
            promotion.EndDate,
            promotion.IsActive);
}
