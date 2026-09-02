using Microsoft.EntityFrameworkCore;
using TaksiApp.Domain.Entities;

namespace TaksiApp.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TaksiApp.Domain.Entities.Address> Addresses => Set<TaksiApp.Domain.Entities.Address>();
    public DbSet<TaksiApp.Domain.Entities.OtpRequest> OtpRequests => Set<TaksiApp.Domain.Entities.OtpRequest>();
    public DbSet<TaksiApp.Domain.Entities.RefreshToken> RefreshTokens => Set<TaksiApp.Domain.Entities.RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = utcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = utcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
