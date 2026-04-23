using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab.Infrastructure.Data.Mappings;

public class RiskControlMapping : IEntityTypeConfiguration<RiskControl>
{
    public void Configure(EntityTypeBuilder<RiskControl> builder)
    {
        builder.Property(x => x.Effectiveness).IsRequired();

        builder.HasOne(x => x.Risk)
            .WithMany()
            .HasForeignKey(x => x.RiskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Control)
          .WithMany()
          .HasForeignKey(x => x.ControlId)
          .OnDelete(DeleteBehavior.Cascade);
    }
}