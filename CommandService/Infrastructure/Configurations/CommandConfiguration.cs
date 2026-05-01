using CommandService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommandService.Infrastructure.Configurations;

public class CommandConfiguration : IEntityTypeConfiguration<Command>
{
    public void Configure(EntityTypeBuilder<Command> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).IsRequired().HasColumnName("id");

        builder.Property(c => c.HowTo).IsRequired().HasColumnName("how_to");

        builder.Property(c => c.CommandLine).IsRequired().HasColumnName("command_line");

        builder.Property(c => c.PlatformId).IsRequired().HasColumnName("platform_id");

        builder.HasOne(c => c.Platform).WithMany(p => p.Commands).HasForeignKey(c => c.PlatformId);
    }
}
