using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.EntityConfigurations;

public class ControlMapping : IEntityTypeConfiguration<Control>
{
    public void Configure(EntityTypeBuilder<Control> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Category).HasConversion(new EnumToStringConverter<EControlCategory>());
        builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<EControlType>());
    }
}