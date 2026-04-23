using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.Mappings;

public class ThreatMapping : IEntityTypeConfiguration<Threat>
{
    public void Configure(EntityTypeBuilder<Threat> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Category).HasConversion(new EnumToStringConverter<EThreatCategory>());
    }
}
