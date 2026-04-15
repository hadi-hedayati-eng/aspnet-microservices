using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlatformService.Domain;

namespace PlatformService.Infrastructure.Configuration;

public class PlatformConfiguration : IEntityTypeConfiguration<Platform>
{
    public void Configure(EntityTypeBuilder<Platform> builder)
    {
        builder.ToTable("platforms");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).IsRequired().HasColumnName("id");

        builder.Property(p => p.Name).IsRequired().HasColumnName("name");
        builder.Property(p => p.Publisher).IsRequired().HasColumnName("publisher");
        builder.Property(p => p.Cost).IsRequired().HasColumnName("cost");
    }
}
