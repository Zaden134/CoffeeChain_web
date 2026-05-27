using CoffeeChainManagement.Domain.Entities;
using CoffeeChainManagement.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Persistence;

// CoffeeChainDbSeeder tao du lieu mau va tai khoan mac dinh cho moi truong dev.
public sealed class CoffeeChainDbSeeder(
    CoffeeChainDbContext dbContext,
    IPasswordHasher<Employee> passwordHasher)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.Branches.AnyAsync(cancellationToken))
        {
            return;
        }

        var branch1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var branch2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var branch3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var espressoId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var latteId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var teaId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

        var coffeeBeansId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var milkId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var peachSyrupId = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");

        var admin = new Employee
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            Username = "admin",
            FullName = "System Administrator",
            Email = "admin@coffeechain.local",
            PasswordHash = string.Empty,
            Role = UserRole.Administrator,
            BranchId = null,
            IsActive = true
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");

        var manager = new Employee
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            Username = "manager.q1",
            FullName = "Nguyen Minh Chau",
            Email = "manager.q1@coffeechain.local",
            PasswordHash = string.Empty,
            Role = UserRole.BranchManager,
            BranchId = branch1Id,
            IsActive = true
        };
        manager.PasswordHash = passwordHasher.HashPassword(manager, "Manager@123");

        var cashier = new Employee
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            Username = "cashier.q1",
            FullName = "Tran Bao Han",
            Email = "cashier.q1@coffeechain.local",
            PasswordHash = string.Empty,
            Role = UserRole.Cashier,
            BranchId = branch1Id,
            IsActive = true
        };
        cashier.PasswordHash = passwordHasher.HashPassword(cashier, "Cashier@123");

        var warehouseStaff = new Employee
        {
            Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            Username = "warehouse.q1",
            FullName = "Pham Duc Anh",
            Email = "warehouse.q1@coffeechain.local",
            PasswordHash = string.Empty,
            Role = UserRole.WarehouseStaff,
            BranchId = branch1Id,
            IsActive = true
        };
        warehouseStaff.PasswordHash = passwordHasher.HashPassword(warehouseStaff, "Warehouse@123");

        dbContext.Branches.AddRange(
            new Branch
            {
                Id = branch1Id,
                Code = "HCM-Q1",
                Name = "Coffee Hub Quan 1",
                Address = "12 Nguyen Hue, Quan 1, TP.HCM",
                ManagerName = "Nguyen Minh Chau",
                IsActive = true
            },
            new Branch
            {
                Id = branch2Id,
                Code = "HN-CG",
                Name = "Coffee Hub Cau Giay",
                Address = "88 Tran Dang Ninh, Cau Giay, Ha Noi",
                ManagerName = "Tran Hoang Nam",
                IsActive = true
            },
            new Branch
            {
                Id = branch3Id,
                Code = "DN-HC",
                Name = "Coffee Hub Hai Chau",
                Address = "25 Bach Dang, Hai Chau, Da Nang",
                ManagerName = "Le Thu Ha",
                IsActive = true
            });

        dbContext.Products.AddRange(
            new Product
            {
                Id = espressoId,
                Sku = "CF-ESP-001",
                Name = "Espresso Signature",
                Category = "Coffee",
                Price = 39000m,
                ImageUrl = "https://images.unsplash.com/photo-1517701604599-bb29b565090c?auto=format&fit=crop&w=600&q=80",
                IsAvailable = true
            },
            new Product
            {
                Id = latteId,
                Sku = "CF-LAT-002",
                Name = "Latte Oat Milk",
                Category = "Coffee",
                Price = 52000m,
                ImageUrl = "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=600&q=80",
                IsAvailable = true
            },
            new Product
            {
                Id = teaId,
                Sku = "TEA-PEA-003",
                Name = "Peach Tea",
                Category = "Tea",
                Price = 45000m,
                ImageUrl = "https://images.unsplash.com/photo-1461023058943-07fcbe16d735?auto=format&fit=crop&w=600&q=80",
                IsAvailable = true
            });

        dbContext.Ingredients.AddRange(
            new Ingredient { Id = coffeeBeansId, Name = "Coffee Beans", Unit = "kg", ReorderLevel = 8m },
            new Ingredient { Id = milkId, Name = "Fresh Milk", Unit = "litre", ReorderLevel = 15m },
            new Ingredient { Id = peachSyrupId, Name = "Peach Syrup", Unit = "bottle", ReorderLevel = 5m });

        dbContext.InventoryItems.AddRange(
            new InventoryItem { BranchId = branch1Id, IngredientId = coffeeBeansId, InStockQuantity = 10m, ReservedQuantity = 1m },
            new InventoryItem { BranchId = branch1Id, IngredientId = milkId, InStockQuantity = 12m, ReservedQuantity = 1m },
            new InventoryItem { BranchId = branch1Id, IngredientId = peachSyrupId, InStockQuantity = 4m, ReservedQuantity = 0m },
            new InventoryItem { BranchId = branch2Id, IngredientId = coffeeBeansId, InStockQuantity = 14m, ReservedQuantity = 0m },
            new InventoryItem { BranchId = branch2Id, IngredientId = milkId, InStockQuantity = 10m, ReservedQuantity = 0m },
            new InventoryItem { BranchId = branch2Id, IngredientId = peachSyrupId, InStockQuantity = 6m, ReservedQuantity = 0m },
            new InventoryItem { BranchId = branch3Id, IngredientId = coffeeBeansId, InStockQuantity = 7m, ReservedQuantity = 1m },
            new InventoryItem { BranchId = branch3Id, IngredientId = milkId, InStockQuantity = 8m, ReservedQuantity = 0m },
            new InventoryItem { BranchId = branch3Id, IngredientId = peachSyrupId, InStockQuantity = 3m, ReservedQuantity = 0m });

        dbContext.Employees.AddRange(admin, manager, cashier, warehouseStaff);

        dbContext.Promotions.AddRange(
            new Promotion
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000001"),
                Name = "Happy Morning 15%",
                DiscountPercent = 15m,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10)),
                IsActive = true
            },
            new Promotion
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                Name = "Tea Combo 10%",
                DiscountPercent = 10m,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-2)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
                IsActive = true
            });

        dbContext.RecruitmentRequests.Add(
            new RecruitmentRequest
            {
                Id = Guid.Parse("50000000-0000-0000-0000-000000000001"),
                BranchId = branch1Id,
                RequestedByEmployeeId = manager.Id,
                PositionTitle = "Cashier full-time",
                Quantity = 2,
                Reason = "Chi nhanh Quan 1 can bo sung nhan su cho khung gio toi va cuoi tuan.",
                Status = RecruitmentRequestStatus.Pending
            });

        dbContext.SaleOrders.AddRange(
            new SaleOrder
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000001"),
                BranchId = branch1Id,
                EmployeeId = cashier.Id,
                PaymentMethod = PaymentMethod.Cash,
                Status = OrderStatus.Paid,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-2),
                Items =
                [
                    new SaleOrderItem { ProductId = espressoId, ProductName = "Espresso Signature", Quantity = 12, UnitPrice = 39000m },
                    new SaleOrderItem { ProductId = latteId, ProductName = "Latte Oat Milk", Quantity = 7, UnitPrice = 52000m }
                ]
            },
            new SaleOrder
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000002"),
                BranchId = branch2Id,
                EmployeeId = manager.Id,
                PaymentMethod = PaymentMethod.Card,
                Status = OrderStatus.Paid,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-4),
                Items =
                [
                    new SaleOrderItem { ProductId = latteId, ProductName = "Latte Oat Milk", Quantity = 10, UnitPrice = 52000m },
                    new SaleOrderItem { ProductId = teaId, ProductName = "Peach Tea", Quantity = 8, UnitPrice = 45000m }
                ]
            },
            new SaleOrder
            {
                Id = Guid.Parse("30000000-0000-0000-0000-000000000003"),
                BranchId = branch3Id,
                EmployeeId = cashier.Id,
                PaymentMethod = PaymentMethod.EWallet,
                Status = OrderStatus.Paid,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-1),
                Items =
                [
                    new SaleOrderItem { ProductId = espressoId, ProductName = "Espresso Signature", Quantity = 5, UnitPrice = 39000m },
                    new SaleOrderItem { ProductId = teaId, ProductName = "Peach Tea", Quantity = 12, UnitPrice = 45000m }
                ]
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
