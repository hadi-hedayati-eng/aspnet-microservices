using CommandService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CommandService.Infrastructure.Configurations;

public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
{
    public void Configure(EntityTypeBuilder<Platform> builder)
    {
        builder.ToTable("platforms");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().HasColumnName("id");

        builder.Property(p => p.Name).HasMaxLength(200).IsRequired().HasColumnName("name");

        builder.Property(p => p.ExternalId).IsRequired().HasColumnName("external_id");

        builder
            .HasMany(p => p.Commands)
            .WithOne(c => c.Platform)
            .HasForeignKey(c => c.PlatformId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
