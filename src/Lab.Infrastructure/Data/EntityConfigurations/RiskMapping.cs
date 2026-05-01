using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.EntityConfigurations;

public class RiskMapping : IEntityTypeConfiguration<Risk>
{
    public void Configure(EntityTypeBuilder<Risk> builder)
    {
        builder.HasOne(x => x.Asset)
            .WithMany(x => x.Risks)
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Threat)
            .WithMany(x => x.Risks)
            .IsRequired()
            .HasForeignKey(x => x.ThreatId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Vulnerability)
            .WithMany()
            .HasForeignKey(x => x.VulnerabilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.AssetId).IsRequired();
        builder.Property(x => x.ThreatId).IsRequired();
        builder.Property(x => x.VulnerabilityId).IsRequired();

        builder.Property(x => x.Probability).IsRequired();
        builder.Property(x => x.Impact).IsRequired();

        builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<ERiskStatus>());

        // Unique name per tenant
        builder.HasIndex(x => new { x.TenantId, x.AssetId, x.ThreatId, x.VulnerabilityId }).IsUnique();
    }
}