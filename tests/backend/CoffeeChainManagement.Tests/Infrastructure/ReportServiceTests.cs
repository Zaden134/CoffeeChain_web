using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using CoffeeChainManagement.Infrastructure.Persistence;
using CoffeeChainManagement.Infrastructure.Services;
using CoffeeChainManagement.Tests.TestDoubles;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Tests.Infrastructure;

public sealed class ReportServiceTests
{
    [Fact]
    public async Task Sales_report_filters_and_exports()
    {
        await using var db = CreateDb();
        var branch = new Branch
        {
            Id = Guid.NewGuid(),
            Code = "B1",
            Name = "Branch 1",
            Address = "Addr 1",
            ManagerName = "Manager 1",
            IsActive = true
        };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Sku = "SKU-1",
            Name = "Latte",
            Category = "Coffee",
            Price = 50000m,
            ImageUrl = "",
            IsAvailable = true
        };
        db.Branches.Add(branch);
        db.Products.Add(product);
        db.SaleOrders.Add(new SaleOrder
        {
            Id = Guid.NewGuid(),
            BranchId = branch.Id,
            EmployeeId = Guid.NewGuid(),
            PaymentMethod = PaymentMethod.Cash,
            Status = OrderStatus.Paid,
            CreatedAtUtc = DateTime.UtcNow.Date.AddHours(8),
            Items =
            [
                new SaleOrderItem { ProductId = product.Id, ProductName = product.Name, Quantity = 2, UnitPrice = 50000m }
            ]
        });
        await db.SaveChangesAsync();

        var currentUser = new TestCurrentUserContext(Guid.NewGuid(), "admin", null, UserRole.Administrator);
        var service = new PostgresReportService(db, currentUser);
        var report = await service.GetSalesReportAsync(null, null, branch.Id);
        var export = await service.ExportSalesReportAsync(null, null, branch.Id, "xlsx");

        Assert.Equal(100000m, report.TotalRevenue);
        Assert.Equal(1, report.TotalOrders);
        Assert.Equal("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", export.ContentType);
        Assert.NotEmpty(export.Content);
    }

    [Fact]
    public async Task Branch_manager_cannot_access_other_branch_report()
    {
        await using var db = CreateDb();
        var ownBranchId = Guid.NewGuid();
        var otherBranchId = Guid.NewGuid();
        var currentUser = new TestCurrentUserContext(Guid.NewGuid(), "manager.q1", ownBranchId, UserRole.BranchManager);
        var service = new PostgresReportService(db, currentUser);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.GetSalesReportAsync(null, null, otherBranchId));
    }

    private static CoffeeChainDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<CoffeeChainDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new CoffeeChainDbContext(options);
    }
}
