using CoffeeChainManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Persistence;

// CoffeeChainDbContext la diem noi chinh giua EF Core va PostgreSQL.
public sealed class CoffeeChainDbContext(DbContextOptions<CoffeeChainDbContext> options) : DbContext(options)
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<SaleOrder> SaleOrders => Set<SaleOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("branches");
            entity.HasKey(branch => branch.Id);
            entity.Property(branch => branch.Code).HasMaxLength(30);
            entity.Property(branch => branch.Name).HasMaxLength(150);
            entity.Property(branch => branch.Address).HasMaxLength(300);
            entity.Property(branch => branch.ManagerName).HasMaxLength(150);
            entity.HasIndex(branch => branch.Code).IsUnique();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(product => product.Id);
            entity.Property(product => product.Sku).HasMaxLength(50);
            entity.Property(product => product.Name).HasMaxLength(150);
            entity.Property(product => product.Category).HasMaxLength(100);
            entity.Property(product => product.Price).HasPrecision(18, 2);
            entity.Property(product => product.ImageUrl).HasMaxLength(500);
            entity.HasIndex(product => product.Sku).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");
            entity.HasKey(employee => employee.Id);
            entity.Property(employee => employee.Username).HasMaxLength(50);
            entity.Property(employee => employee.FullName).HasMaxLength(150);
            entity.Property(employee => employee.Email).HasMaxLength(150);
            entity.Property(employee => employee.PasswordHash).HasMaxLength(1000);
            entity.HasIndex(employee => employee.Username).IsUnique();
            entity.HasIndex(employee => employee.Email).IsUnique();
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.ToTable("ingredients");
            entity.HasKey(ingredient => ingredient.Id);
            entity.Property(ingredient => ingredient.Name).HasMaxLength(150);
            entity.Property(ingredient => ingredient.Unit).HasMaxLength(20);
            entity.Property(ingredient => ingredient.ReorderLevel).HasPrecision(18, 2);
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.ToTable("inventory_items");
            entity.HasKey(item => item.Id);
            entity.Property(item => item.InStockQuantity).HasPrecision(18, 2);
            entity.Property(item => item.ReservedQuantity).HasPrecision(18, 2);
            entity.HasIndex(item => new { item.BranchId, item.IngredientId }).IsUnique();
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("promotions");
            entity.HasKey(promotion => promotion.Id);
            entity.Property(promotion => promotion.Name).HasMaxLength(150);
            entity.Property(promotion => promotion.DiscountPercent).HasPrecision(5, 2);
        });

        modelBuilder.Entity<SaleOrder>(entity =>
        {
            entity.ToTable("sale_orders");
            entity.HasKey(order => order.Id);
            entity.Ignore(order => order.TotalAmount);
            entity.OwnsMany(
                order => order.Items,
                item =>
                {
                    item.ToTable("sale_order_items");
                    item.WithOwner().HasForeignKey("SaleOrderId");
                    item.Property<Guid>("Id").ValueGeneratedOnAdd();
                    item.HasKey("Id");
                    item.Property(orderItem => orderItem.ProductName).HasMaxLength(150);
                    item.Property(orderItem => orderItem.UnitPrice).HasPrecision(18, 2);
                });
        });
    }
}
