namespace CoffeeChainManagement.Infrastructure.Persistence;

// IDatabaseInitializer tach ro step migrate schema va step seed data.
public interface IDatabaseInitializer
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
    Task SeedAsync(CancellationToken cancellationToken = default);
    Task RevokeActiveAuthSessionsAsync(CancellationToken cancellationToken = default);
}
