using Microsoft.EntityFrameworkCore;
using PlatformService.Domain;

namespace PlatformService.Infrastructure;

public class PlatformServiceDbContext : DbContext
{
    public PlatformServiceDbContext(DbContextOptions<PlatformServiceDbContext> options)
        : base(options) { }

    public DbSet<Platform> Platforms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlatformServiceDbContext).Assembly);
    }
}
