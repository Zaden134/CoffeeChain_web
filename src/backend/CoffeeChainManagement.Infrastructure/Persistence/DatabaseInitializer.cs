using Microsoft.EntityFrameworkCore;

namespace CoffeeChainManagement.Infrastructure.Persistence;

// DatabaseInitializer dieu phoi migrate va seed de startup ro trach nhiem hon.
public sealed class DatabaseInitializer(
    CoffeeChainDbContext dbContext,
    CoffeeChainDbSeeder seeder) : IDatabaseInitializer
{
    public Task MigrateAsync(CancellationToken cancellationToken = default)
        => dbContext.Database.MigrateAsync(cancellationToken);

    public Task SeedAsync(CancellationToken cancellationToken = default)
        => seeder.SeedAsync(cancellationToken);
}
