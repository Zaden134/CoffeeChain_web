using CoffeeChainManagement.Application.DTOs.Branches;
using CoffeeChainManagement.Application.DTOs.Dashboard;
using CoffeeChainManagement.Application.DTOs.Products;

namespace CoffeeChainManagement.Infrastructure.Persistence;

// InMemorySeedData la du lieu mau de team co API chay ngay truoc khi noi PostgreSQL that.
internal static class InMemorySeedData
{
    internal static readonly IReadOnlyCollection<BranchSummaryDto> Branches =
    [
        new(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "HCM-Q1",
            "Coffee Hub Quan 1",
            "12 Nguyen Hue, Quan 1, TP.HCM",
            "Nguyen Minh Chau",
            18500000m,
            2,
            true),
        new(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "HN-CG",
            "Coffee Hub Cau Giay",
            "88 Tran Dang Ninh, Cau Giay, Ha Noi",
            "Tran Hoang Nam",
            13400000m,
            1,
            true),
        new(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "DN-HC",
            "Coffee Hub Hai Chau",
            "25 Bach Dang, Hai Chau, Da Nang",
            "Le Thu Ha",
            9800000m,
            4,
            true)
    ];

    internal static readonly IReadOnlyCollection<ProductSummaryDto> Products =
    [
        new(
            Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            "CF-ESP-001",
            "Espresso Signature",
            "Coffee",
            39000m,
            "https://images.unsplash.com/photo-1517701604599-bb29b565090c?auto=format&fit=crop&w=600&q=80",
            true,
            128),
        new(
            Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            "CF-LAT-002",
            "Latte Oat Milk",
            "Coffee",
            52000m,
            "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=600&q=80",
            true,
            94),
        new(
            Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            "TEA-PEA-003",
            "Peach Tea",
            "Tea",
            45000m,
            "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=600&q=80",
            true,
            76)
    ];

    internal static readonly DashboardOverviewDto DashboardOverview =
        new(
            new KpiSummaryDto(
                DailyRevenue: 41700000m,
                MonthlyRevenue: 1123000000m,
                TotalOrders: 926,
                ActiveBranches: 3,
                LowStockAlerts: 7),
            Branches.OrderByDescending(branch => branch.RevenueToday).Take(3).ToArray(),
            Products.OrderByDescending(product => product.SoldQuantity).Take(3).ToArray());
}
