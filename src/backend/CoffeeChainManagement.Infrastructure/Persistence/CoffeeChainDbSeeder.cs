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
        var branch1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var branch2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var branch3Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

        if (await dbContext.Branches.AnyAsync(cancellationToken))
        {
            var fallbackBranchId = await ResolveSeedBranchIdAsync(branch1Id, cancellationToken);
            await EnsureSeedProductsAsync(cancellationToken);
            await EnsureSeedRecipesAndInventoryAsync(cancellationToken);
            await EnsureSeedEmployeeAsync(
                Guid.Parse("10000000-0000-0000-0000-000000000001"),
                "admin",
                "System Administrator",
                "admin@coffeechain.local",
                "Admin@123",
                UserRole.Administrator,
                null,
                cancellationToken);
            await EnsureSeedEmployeeAsync(
                Guid.Parse("10000000-0000-0000-0000-000000000002"),
                "manager.q1",
                "Nguyen Minh Chau",
                "manager.q1@coffeechain.local",
                "Manager@123",
                UserRole.BranchManager,
                fallbackBranchId,
                cancellationToken);
            await EnsureSeedEmployeeAsync(
                Guid.Parse("10000000-0000-0000-0000-000000000003"),
                "cashier.q1",
                "Tran Bao Han",
                "cashier.q1@coffeechain.local",
                "Cashier@123",
                UserRole.Cashier,
                fallbackBranchId,
                cancellationToken);
            await EnsureSeedEmployeeAsync(
                Guid.Parse("10000000-0000-0000-0000-000000000004"),
                "warehouse.q1",
                "Pham Duc Anh",
                "warehouse.q1@coffeechain.local",
                "Warehouse@123",
                UserRole.WarehouseStaff,
                fallbackBranchId,
                cancellationToken);
            await EnsureSeedInventoryTransactionsAsync(fallbackBranchId, cancellationToken);
            await EnsureSeedSaleOrdersAsync(cancellationToken);
            await EnsureInventoryImportAmountsAsync(cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

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
                Code = "HAPPY15",
                Name = "Happy Morning 15%",
                DiscountPercent = 15m,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-5)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10)),
                IsActive = true
            },
            new Promotion
            {
                Id = Guid.Parse("40000000-0000-0000-0000-000000000002"),
                Code = "TEA10",
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

        dbContext.InventoryTransactions.AddRange(
            new InventoryTransaction
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                BranchId = branch1Id,
                IngredientId = coffeeBeansId,
                Type = TransactionType.Import,
                Quantity = 18,
                UnitCost = 120000m,
                TransactionAmount = -2160000m,
                ReferenceNumber = "PN-Q1-001",
                Notes = "Nhap hat ca phe cho chi nhanh Q1",
                CreatedBy = warehouseStaff.Id,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-5)
            },
            new InventoryTransaction
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                BranchId = branch1Id,
                IngredientId = milkId,
                Type = TransactionType.Import,
                Quantity = 25,
                UnitCost = 32000m,
                TransactionAmount = -800000m,
                ReferenceNumber = "PN-Q1-002",
                Notes = "Nhap sua tuoi",
                CreatedBy = warehouseStaff.Id,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-3)
            },
            new InventoryTransaction
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000003"),
                BranchId = branch1Id,
                IngredientId = peachSyrupId,
                Type = TransactionType.Export,
                Quantity = -3,
                UnitCost = 0m,
                TransactionAmount = 0m,
                ReferenceNumber = "PX-Q1-001",
                Notes = "Xuat syrup cho ca chieu",
                CreatedBy = warehouseStaff.Id,
                CreatedAtUtc = DateTime.UtcNow.AddHours(-1)
            });

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Guid> ResolveSeedBranchIdAsync(Guid preferredBranchId, CancellationToken cancellationToken)
        => await dbContext.Branches.AnyAsync(branch => branch.Id == preferredBranchId, cancellationToken)
            ? preferredBranchId
            : await dbContext.Branches
                .OrderBy(branch => branch.CreatedAtUtc)
                .Select(branch => branch.Id)
                .FirstAsync(cancellationToken);

    private async Task EnsureSeedEmployeeAsync(
        Guid id,
        string username,
        string fullName,
        string email,
        string password,
        UserRole role,
        Guid? branchId,
        CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees.SingleOrDefaultAsync(item => item.Username == username, cancellationToken);
        if (employee is null)
        {
            employee = new Employee
            {
                Id = id,
                Username = username,
                FullName = fullName,
                Email = email,
                PasswordHash = string.Empty,
                Role = role,
                BranchId = branchId,
                IsActive = true
            };
            dbContext.Employees.Add(employee);
        }
        else
        {
            employee.FullName = fullName;
            employee.Email = email;
            employee.Role = role;
            employee.BranchId = branchId;
            employee.IsActive = true;
            employee.UpdatedAtUtc = DateTime.UtcNow;
        }

        employee.PasswordHash = passwordHasher.HashPassword(employee, password);
    }

    private async Task EnsureSeedInventoryTransactionsAsync(Guid branchId, CancellationToken cancellationToken)
    {
        var coffeeBeansId = await dbContext.Ingredients
            .Where(ingredient => ingredient.Name == "Coffee Beans")
            .Select(ingredient => ingredient.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var milkId = await dbContext.Ingredients
            .Where(ingredient => ingredient.Name == "Fresh Milk")
            .Select(ingredient => ingredient.Id)
            .FirstOrDefaultAsync(cancellationToken);
        var warehouseStaffId = await dbContext.Employees
            .Where(employee => employee.Username == "warehouse.q1")
            .Select(employee => employee.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (coffeeBeansId == Guid.Empty || milkId == Guid.Empty || warehouseStaffId == Guid.Empty)
        {
            return;
        }

        var seedTransactions = new[]
        {
            new
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000001"),
                IngredientId = coffeeBeansId,
                Type = TransactionType.Import,
                Quantity = 18,
                UnitCost = 120000m,
                ReferenceNumber = "PN-Q1-001",
                Notes = "Nhap hat ca phe cho chi nhanh Q1",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-5)
            },
            new
            {
                Id = Guid.Parse("60000000-0000-0000-0000-000000000002"),
                IngredientId = milkId,
                Type = TransactionType.Import,
                Quantity = 25,
                UnitCost = 32000m,
                ReferenceNumber = "PN-Q1-002",
                Notes = "Nhap sua tuoi",
                CreatedAtUtc = DateTime.UtcNow.AddHours(-3)
            }
        };

        foreach (var seed in seedTransactions)
        {
            var exists = await dbContext.InventoryTransactions
                .AnyAsync(transaction => transaction.ReferenceNumber == seed.ReferenceNumber, cancellationToken);
            if (exists)
            {
                continue;
            }

            dbContext.InventoryTransactions.Add(new InventoryTransaction
            {
                Id = seed.Id,
                BranchId = branchId,
                IngredientId = seed.IngredientId,
                Type = seed.Type,
                Quantity = seed.Quantity,
                UnitCost = seed.UnitCost,
                TransactionAmount = seed.Type == TransactionType.Import ? -(seed.UnitCost * seed.Quantity) : 0,
                ReferenceNumber = seed.ReferenceNumber,
                Notes = seed.Notes,
                CreatedBy = warehouseStaffId,
                CreatedAtUtc = seed.CreatedAtUtc
            });
        }
    }

    private async Task EnsureInventoryImportAmountsAsync(CancellationToken cancellationToken)
    {
        var imports = await dbContext.InventoryTransactions
            .Where(transaction => transaction.Type == TransactionType.Import)
            .ToListAsync(cancellationToken);

        foreach (var transaction in imports)
        {
            var quantity = Math.Abs(transaction.Quantity);
            var expectedAmount = -(transaction.UnitCost * quantity);
            if (transaction.Quantity < 0)
            {
                transaction.Quantity = quantity;
            }

            if (transaction.TransactionAmount != expectedAmount)
            {
                transaction.TransactionAmount = expectedAmount;
                transaction.UpdatedAtUtc = DateTime.UtcNow;
            }
        }
    }

    private async Task EnsureSeedSaleOrdersAsync(CancellationToken cancellationToken)
    {
        var todayUtc = DateTime.UtcNow.Date;
        var hasOrdersToday = await dbContext.SaleOrders
            .AnyAsync(order => order.Status == OrderStatus.Paid && order.CreatedAtUtc >= todayUtc, cancellationToken);
        if (hasOrdersToday)
        {
            return;
        }

        var branches = await dbContext.Branches
            .Where(branch => branch.IsActive)
            .OrderBy(branch => branch.Code)
            .Take(3)
            .ToListAsync(cancellationToken);
        var products = await dbContext.Products
            .Where(product => product.IsAvailable)
            .OrderBy(product => product.Sku)
            .Take(3)
            .ToListAsync(cancellationToken);
        var employeeId = await dbContext.Employees
            .Where(employee => employee.Username == "cashier.q1" || employee.Username == "manager.q1" || employee.Username == "admin")
            .OrderBy(employee => employee.Role)
            .Select(employee => employee.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (branches.Count == 0 || products.Count == 0 || employeeId == Guid.Empty)
        {
            return;
        }

        for (var index = 0; index < branches.Count; index++)
        {
            var firstProduct = products[index % products.Count];
            var secondProduct = products[(index + 1) % products.Count];
            dbContext.SaleOrders.Add(new SaleOrder
            {
                Id = Guid.NewGuid(),
                BranchId = branches[index].Id,
                EmployeeId = employeeId,
                PaymentMethod = index % 2 == 0 ? PaymentMethod.Cash : PaymentMethod.Card,
                Status = OrderStatus.Paid,
                CreatedAtUtc = todayUtc.AddHours(8 + (index * 2)),
                Items =
                [
                    new SaleOrderItem
                    {
                        ProductId = firstProduct.Id,
                        ProductName = firstProduct.Name,
                        Quantity = 6 + index,
                        UnitPrice = firstProduct.Price
                    },
                    new SaleOrderItem
                    {
                        ProductId = secondProduct.Id,
                        ProductName = secondProduct.Name,
                        Quantity = 3 + index,
                        UnitPrice = secondProduct.Price
                    }
                ]
            });
        }
    }

    private async Task EnsureSeedProductsAsync(CancellationToken cancellationToken)
    {
        var existingSkus = await dbContext.Products.Select(p => p.Sku).ToListAsync(cancellationToken);
        
        var productsToSeed = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Sku = "CF-BX-01", Name = "Bạc Xỉu", Category = "Coffee", Price = 35000m, ImageUrl = "https://images.unsplash.com/photo-1572442388796-11668a67e53d?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "CF-DD-02", Name = "Cà Phê Đen Đá", Category = "Coffee", Price = 29000m, ImageUrl = "https://images.unsplash.com/photo-1514432324607-a128b3715f58?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "CF-SD-03", Name = "Cà Phê Sữa Đá", Category = "Coffee", Price = 35000m, ImageUrl = "https://images.unsplash.com/photo-1550585640-7e3f847d0663?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "CF-LM-04", Name = "Latte Macchiato", Category = "Coffee", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1570968915860-54d5c301fa9f?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "CF-CP-05", Name = "Cappuccino", Category = "Coffee", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1534778101976-62847782c213?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "CF-AM-06", Name = "Americano", Category = "Coffee", Price = 45000m, ImageUrl = "https://images.unsplash.com/photo-1551030173-122aabc4489c?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            
            new Product { Id = Guid.NewGuid(), Sku = "MT-TCD-01", Name = "Trà Sữa Trân Châu Đen", Category = "Milk Tea", Price = 45000m, ImageUrl = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "MT-TX-02", Name = "Trà Sữa Thái Xanh", Category = "Milk Tea", Price = 45000m, ImageUrl = "https://images.unsplash.com/photo-1558160074-4d7d8bdf4256?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "MT-OL-03", Name = "Trà Sữa Oolong Lộc Đỉnh", Category = "Milk Tea", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1622614569080-b7dc4468f0a0?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "MT-MC-04", Name = "Trà Sữa Matcha", Category = "Milk Tea", Price = 50000m, ImageUrl = "https://images.unsplash.com/photo-1582787031149-14a51eb8c9cb?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "MT-HN-05", Name = "Trà Sữa Hạnh Nhân", Category = "Milk Tea", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1595180554585-703d15db157d?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            
            new Product { Id = Guid.NewGuid(), Sku = "TE-DCS-01", Name = "Trà Đào Cam Sả", Category = "Tea", Price = 49000m, ImageUrl = "https://images.unsplash.com/photo-1499638673689-79a0b5115d87?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "TE-VND-02", Name = "Trà Vải Nhiệt Đới", Category = "Tea", Price = 49000m, ImageUrl = "https://images.unsplash.com/photo-1519623286359-e9f3cbef015b?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "TE-OSV-03", Name = "Trà Oolong Sen Vàng", Category = "Tea", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1563227812-0ea4c22e6cc8?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "TE-HCM-04", Name = "Trà Hoa Cúc Mật Ong", Category = "Tea", Price = 45000m, ImageUrl = "https://images.unsplash.com/photo-1596489311166-70139b4b0e9a?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "TE-LMC-05", Name = "Trà Lài Macchiato", Category = "Tea", Price = 50000m, ImageUrl = "https://images.unsplash.com/photo-1576092768241-dec231879fc3?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            
            new Product { Id = Guid.NewGuid(), Sku = "JU-DH-01", Name = "Nước Ép Dưa Hấu", Category = "Juice", Price = 40000m, ImageUrl = "https://images.unsplash.com/photo-1595981267035-7b04d84b5c7f?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "JU-CA-02", Name = "Nước Ép Cam", Category = "Juice", Price = 45000m, ImageUrl = "https://images.unsplash.com/photo-1613478223719-2ab802602423?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "JU-TC-03", Name = "Nước Ép Táo Cần Tây", Category = "Juice", Price = 50000m, ImageUrl = "https://images.unsplash.com/photo-1600271886742-f049cd451bba?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "JU-TH-04", Name = "Nước Ép Thơm", Category = "Juice", Price = 40000m, ImageUrl = "https://images.unsplash.com/photo-1502741224143-9038af820c74?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            
            new Product { Id = Guid.NewGuid(), Sku = "SM-BO-01", Name = "Sinh Tố Bơ", Category = "Smoothie", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1626343510619-3c74c93f0b09?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "SM-XO-02", Name = "Sinh Tố Xoài", Category = "Smoothie", Price = 50000m, ImageUrl = "https://images.unsplash.com/photo-1596637841972-e160a0f025e1?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "SM-DA-03", Name = "Sinh Tố Dâu", Category = "Smoothie", Price = 55000m, ImageUrl = "https://images.unsplash.com/photo-1553530666-ba11a7da3888?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            new Product { Id = Guid.NewGuid(), Sku = "SM-MC-04", Name = "Sinh Tố Mãng Cầu", Category = "Smoothie", Price = 50000m, ImageUrl = "https://images.unsplash.com/photo-1623065422900-05832a2f8b5f?auto=format&fit=crop&w=600&q=80", IsAvailable = true },
            
            new Product { Id = Guid.NewGuid(), Sku = "IB-SC-01", Name = "Đá Xay Socola", Category = "Ice Blended", Price = 59000m, ImageUrl = "https://images.unsplash.com/photo-1572490122747-3968b75bb8ef?auto=format&fit=crop&w=600&q=80", IsAvailable = true }
        };

        var productsToAdd = productsToSeed.Where(p => !existingSkus.Contains(p.Sku)).ToList();
        if (productsToAdd.Any())
        {
            dbContext.Products.AddRange(productsToAdd);
        }
    }

    private async Task EnsureSeedRecipesAndInventoryAsync(CancellationToken cancellationToken)
    {
        var requiredIngredients = new Dictionary<string, string> // Name -> Unit
        {
            { "Trà đen", "g" },
            { "Trà xanh", "g" },
            { "Sữa đặc", "ml" },
            { "Trân châu đen", "g" },
            { "Bột Matcha", "g" },
            { "Sữa hạnh nhân", "ml" },
            { "Cam tươi", "quả" },
            { "Dưa hấu", "kg" },
            { "Thơm", "quả" },
            { "Táo", "quả" },
            { "Cần tây", "g" },
            { "Bơ", "kg" },
            { "Xoài", "kg" },
            { "Dâu tây", "kg" },
            { "Mãng cầu", "kg" },
            { "Socola", "g" },
            { "Đá viên", "g" }
        };

        var existingIngredients = await dbContext.Ingredients.ToDictionaryAsync(i => i.Name, i => i, cancellationToken);
        
        foreach (var req in requiredIngredients)
        {
            if (!existingIngredients.ContainsKey(req.Key))
            {
                var newIng = new Ingredient { Id = Guid.NewGuid(), Name = req.Key, Unit = req.Value, ReorderLevel = 10m };
                dbContext.Ingredients.Add(newIng);
                existingIngredients[req.Key] = newIng;
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        // Define recipes mapping Product SKU -> Dictionary<IngredientName, RequiredQty>
        var recipeDefs = new Dictionary<string, Dictionary<string, int>>
        {
            { "CF-BX-01", new() { { "Coffee Beans", 15 }, { "Sữa đặc", 40 }, { "Fresh Milk", 20 }, { "Đá viên", 100 } } },
            { "CF-DD-02", new() { { "Coffee Beans", 20 }, { "Đá viên", 100 } } },
            { "CF-SD-03", new() { { "Coffee Beans", 20 }, { "Sữa đặc", 30 }, { "Đá viên", 100 } } },
            { "CF-LM-04", new() { { "Coffee Beans", 10 }, { "Fresh Milk", 150 } } },
            { "CF-CP-05", new() { { "Coffee Beans", 10 }, { "Fresh Milk", 120 } } },
            { "CF-AM-06", new() { { "Coffee Beans", 20 } } },
            
            { "MT-TCD-01", new() { { "Trà đen", 10 }, { "Sữa đặc", 20 }, { "Fresh Milk", 50 }, { "Trân châu đen", 50 }, { "Đá viên", 100 } } },
            { "MT-TX-02", new() { { "Trà xanh", 10 }, { "Sữa đặc", 30 }, { "Trân châu đen", 50 }, { "Đá viên", 100 } } },
            { "MT-OL-03", new() { { "Trà đen", 10 }, { "Sữa đặc", 20 }, { "Fresh Milk", 40 }, { "Đá viên", 100 } } },
            { "MT-MC-04", new() { { "Bột Matcha", 5 }, { "Fresh Milk", 100 }, { "Đá viên", 100 } } },
            { "MT-HN-05", new() { { "Trà đen", 10 }, { "Sữa hạnh nhân", 100 }, { "Đá viên", 100 } } },
            
            { "TE-DCS-01", new() { { "Trà đen", 10 }, { "Peach Syrup", 20 }, { "Cam tươi", 1 }, { "Đá viên", 100 } } },
            { "TE-VND-02", new() { { "Trà đen", 10 }, { "Đá viên", 100 } } },
            { "TE-OSV-03", new() { { "Trà xanh", 10 }, { "Đá viên", 100 } } },
            { "TE-HCM-04", new() { { "Trà xanh", 10 }, { "Đá viên", 100 } } },
            { "TE-LMC-05", new() { { "Trà xanh", 10 }, { "Fresh Milk", 50 }, { "Đá viên", 100 } } },
            
            { "JU-DH-01", new() { { "Dưa hấu", 1 }, { "Đá viên", 50 } } },
            { "JU-CA-02", new() { { "Cam tươi", 2 }, { "Đá viên", 50 } } },
            { "JU-TC-03", new() { { "Táo", 1 }, { "Cần tây", 50 }, { "Đá viên", 50 } } },
            { "JU-TH-04", new() { { "Thơm", 1 }, { "Đá viên", 50 } } },
            
            { "SM-BO-01", new() { { "Bơ", 1 }, { "Sữa đặc", 20 }, { "Fresh Milk", 50 }, { "Đá viên", 100 } } },
            { "SM-XO-02", new() { { "Xoài", 1 }, { "Sữa đặc", 20 }, { "Fresh Milk", 50 }, { "Đá viên", 100 } } },
            { "SM-DA-03", new() { { "Dâu tây", 1 }, { "Sữa đặc", 20 }, { "Fresh Milk", 50 }, { "Đá viên", 100 } } },
            { "SM-MC-04", new() { { "Mãng cầu", 1 }, { "Sữa đặc", 20 }, { "Fresh Milk", 50 }, { "Đá viên", 100 } } },
            
            { "IB-SC-01", new() { { "Socola", 30 }, { "Fresh Milk", 80 }, { "Đá viên", 150 } } }
        };

        var products = await dbContext.Products.ToListAsync(cancellationToken);
        
        foreach (var def in recipeDefs)
        {
            var product = products.FirstOrDefault(p => p.Sku == def.Key);
            if (product != null)
            {
                var recipe = await dbContext.Recipes.Include(r => r.Ingredients).FirstOrDefaultAsync(r => r.ProductId == product.Id, cancellationToken);
                if (recipe == null)
                {
                    recipe = new Recipe { Id = Guid.NewGuid(), ProductId = product.Id, Instructions = "Pha chế theo chuẩn." };
                    dbContext.Recipes.Add(recipe);
                }
                
                foreach (var ingReq in def.Value)
                {
                    if (existingIngredients.TryGetValue(ingReq.Key, out var ingredient))
                    {
                        if (!recipe.Ingredients.Any(ri => ri.IngredientId == ingredient.Id))
                        {
                            recipe.Ingredients.Add(new RecipeIngredient { Id = Guid.NewGuid(), RecipeId = recipe.Id, IngredientId = ingredient.Id, RequiredQuantity = ingReq.Value });
                        }
                    }
                }
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        var branches = await dbContext.Branches.ToListAsync(cancellationToken);
        var adminStaff = await dbContext.Employees.FirstOrDefaultAsync(e => e.Username == "admin", cancellationToken);
        var createdById = adminStaff?.Id ?? Guid.Empty;

        foreach (var branch in branches)
        {
            foreach (var ing in existingIngredients.Values)
            {
                var invItem = await dbContext.InventoryItems.FirstOrDefaultAsync(i => i.BranchId == branch.Id && i.IngredientId == ing.Id, cancellationToken);
                if (invItem == null)
                {
                    invItem = new InventoryItem { BranchId = branch.Id, IngredientId = ing.Id, InStockQuantity = 5000m, ReservedQuantity = 0m };
                    dbContext.InventoryItems.Add(invItem);
                    
                    dbContext.InventoryTransactions.Add(new InventoryTransaction
                    {
                        Id = Guid.NewGuid(),
                        BranchId = branch.Id,
                        IngredientId = ing.Id,
                        Type = TransactionType.Import,
                        Quantity = 5000,
                        UnitCost = 10000m,
                        TransactionAmount = -50000000m,
                        ReferenceNumber = $"INIT-{branch.Code}-{ing.Name}",
                        Notes = "Nhập kho ban đầu cho nguyên liệu mới",
                        CreatedBy = createdById,
                        CreatedAtUtc = DateTime.UtcNow
                    });
                }
            }
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
