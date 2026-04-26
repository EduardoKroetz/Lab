using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.EntityConfigurations;

public class RiskControlMapping : IEntityTypeConfiguration<RiskControl>
{
    public void Configure(EntityTypeBuilder<RiskControl> builder)
    {
        builder.Property(x => x.Effectiveness).IsRequired();
        builder.Property(x => x.ControlType).HasConversion(new EnumToStringConverter<EControlType>());

        builder.HasOne<Risk>()
            .WithMany(x => x.RiskControls)
            .HasForeignKey(x => x.RiskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Control)
          .WithMany()
          .HasForeignKey(x => x.ControlId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}