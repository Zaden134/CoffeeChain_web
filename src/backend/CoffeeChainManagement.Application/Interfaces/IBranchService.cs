using CoffeeChainManagement.Application.DTOs.Branches;

namespace CoffeeChainManagement.Application.Interfaces;

// IBranchService la hop dong cho cac use case quan ly chi nhanh.
public interface IBranchService
{
    Task<IReadOnlyCollection<BranchSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
