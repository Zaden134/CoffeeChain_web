using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoffeeChainManagement.Infrastructure.Persistence;

// CoffeeChainDbContextFactory giup dotnet ef tao DbContext khi generate migration.
public sealed class CoffeeChainDbContextFactory : IDesignTimeDbContextFactory<CoffeeChainDbContext>
{
    public CoffeeChainDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CoffeeChainDbContext>();
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=coffee_chain_management;Username=postgres;Password=postgres");

        return new CoffeeChainDbContext(optionsBuilder.Options);
    }
}
