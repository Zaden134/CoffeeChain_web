using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.DTOs.Products;

namespace CoffeeChainManagement.Application.DTOs.Dashboard;

// DashboardOverviewDto gom KPI, chi nhanh noi bat va mon ban chay de frontend ve man hinh tong quan.
public sealed record DashboardOverviewDto(
    KpiSummaryDto Summary,
    IReadOnlyCollection<BranchSummaryDto> TopBranches,
    IReadOnlyCollection<ProductSummaryDto> BestSellingProducts);
