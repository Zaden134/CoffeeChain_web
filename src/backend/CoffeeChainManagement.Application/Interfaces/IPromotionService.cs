using CoffeeChainManagement.Application.DTOs.Promotions;

namespace CoffeeChainManagement.Application.Interfaces;

// IPromotionService dinh nghia CRUD khuyen mai.
public interface IPromotionService
{
    Task<IReadOnlyCollection<PromotionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PromotionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PromotionDto> CreateAsync(UpsertPromotionRequestDto request, CancellationToken cancellationToken = default);
    Task<PromotionDto> UpdateAsync(Guid id, UpsertPromotionRequestDto request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
