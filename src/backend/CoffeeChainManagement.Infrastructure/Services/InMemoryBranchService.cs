using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.Interfaces;
using CoffeeChainManagement.Infrastructure.Persistence;

namespace CoffeeChainManagement.Infrastructure.Services;

// InMemoryBranchService cho phep thay du lieu mau bang repository PostgreSQL sau nay ma khong doi controller.
internal sealed class InMemoryBranchService : IBranchService
{
    public Task<IReadOnlyCollection<BranchSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(InMemorySeedData.Branches);
}
