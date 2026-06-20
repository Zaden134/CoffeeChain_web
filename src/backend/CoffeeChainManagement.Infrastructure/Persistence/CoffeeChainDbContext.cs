using CoffeeChainManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Persistence;

// CoffeeChainDbContext la diem noi chinh giua EF Core va PostgreSQL.
public sealed class CoffeeChainDbContext(DbContextOptions<CoffeeChainDbContext> options) : DbContext(options)
{
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<Promotion> Promotions => Set<Promotion>();
    public DbSet<RefreshTokenSession> RefreshTokenSessions => Set<RefreshTokenSession>();
    public DbSet<RecruitmentRequest> RecruitmentRequests => Set<RecruitmentRequest>();
    public DbSet<SaleOrder> SaleOrders => Set<SaleOrder>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

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

        modelBuilder.Entity<AuditLogEntry>(entity =>
        {
            entity.ToTable("audit_logs");
            entity.HasKey(entry => entry.Id);
            entity.Property(entry => entry.Action).HasMaxLength(120);
            entity.Property(entry => entry.EntityType).HasMaxLength(120);
            entity.Property(entry => entry.EntityId).HasMaxLength(80);
            entity.Property(entry => entry.Details).HasMaxLength(2000);
            entity.Property(entry => entry.Username).HasMaxLength(80);
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

        modelBuilder.Entity<RefreshTokenSession>(entity =>
        {
            entity.ToTable("refresh_token_sessions");
            entity.HasKey(session => session.Id);
            entity.Property(session => session.TokenHash).HasMaxLength(128);
            entity.Property(session => session.ReplacedByTokenHash).HasMaxLength(128);
            entity.Property(session => session.CreatedByIp).HasMaxLength(45);
            entity.Property(session => session.RevokedByIp).HasMaxLength(45);
            entity.HasIndex(session => session.TokenHash).IsUnique();
            entity.HasIndex(session => new { session.EmployeeId, session.ExpiresAtUtc });
        });

        modelBuilder.Entity<RecruitmentRequest>(entity =>
        {
            entity.ToTable("recruitment_requests");
            entity.HasKey(request => request.Id);
            entity.Property(request => request.PositionTitle).HasMaxLength(120);
            entity.Property(request => request.Reason).HasMaxLength(1000);
            entity.Property(request => request.AdminNote).HasMaxLength(1000);
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

        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.ToTable("inventory_transactions");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.ReferenceNumber).HasMaxLength(50);
            entity.Property(t => t.Notes).HasMaxLength(1000);
            entity.HasIndex(t => t.BranchId);
            entity.HasIndex(t => t.IngredientId);
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.ToTable("recipes");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Instructions).HasMaxLength(2000);
            entity.HasIndex(r => r.ProductId).IsUnique();
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.ToTable("recipe_ingredients");
            entity.HasKey(ri => ri.Id);
            entity.HasIndex(ri => new { ri.RecipeId, ri.IngredientId }).IsUnique();
        });
    }
}
