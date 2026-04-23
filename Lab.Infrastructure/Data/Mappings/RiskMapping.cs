using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.Mappings;

public class RiskMapping : IEntityTypeConfiguration<Risk>
{
    public void Configure(EntityTypeBuilder<Risk> builder)
    {
        builder.HasOne(x => x.Asset)
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Threat)
            .WithMany()
            .HasForeignKey(x => x.ThreatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Vulnerability)
            .WithMany()
            .HasForeignKey(x => x.VulnerabilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Probability).IsRequired();
        builder.Property(x => x.Impact).IsRequired();

        builder.Property(x => x.Level).HasConversion(new EnumToStringConverter<ERiskLevel>());
        builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<ERiskStatus>());
    }
}