using CoffeeChainManagement.Infrastructure.Auth;
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

    public async Task RevokeActiveAuthSessionsAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var activeSessions = await dbContext.RefreshTokenSessions
            .Where(session => session.RevokedAtUtc == null && session.ExpiresAtUtc > nowUtc)
            .ToListAsync(cancellationToken);

        foreach (var session in activeSessions)
        {
            session.RevokedAtUtc = nowUtc;
            session.RevokedByIp = ServerSessionMarker.RestartRevocationReason;
            session.UpdatedAtUtc = nowUtc;
        }

        if (activeSessions.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
