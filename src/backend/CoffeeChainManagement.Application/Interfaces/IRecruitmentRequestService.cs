using CoffeeChainManagement.Application.DTOs.Recruitment;

namespace CoffeeChainManagement.Application.Interfaces;

// IRecruitmentRequestService dinh nghia luong manager gui yeu cau va admin phe duyet.
public interface IRecruitmentRequestService
{
    Task<IReadOnlyCollection<RecruitmentRequestDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RecruitmentRequestDto> CreateAsync(CreateRecruitmentRequestDto request, CancellationToken cancellationToken = default);
    Task<RecruitmentRequestDto> ReviewAsync(Guid id, ReviewRecruitmentRequestDto request, CancellationToken cancellationToken = default);
}
