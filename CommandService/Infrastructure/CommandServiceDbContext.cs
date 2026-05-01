using CommandService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Infrastructure;

public class CommandServiceDbContext : DbContext
{
    public DbSet<Platform> Platforms { get; set; }
    public DbSet<Command> Commands { get; set; }

    public CommandServiceDbContext(DbContextOptions<CommandServiceDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommandServiceDbContext).Assembly);
    }
}
