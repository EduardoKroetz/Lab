using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.Mappings;

public class IncidentMapping : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.DateOccurred).IsRequired();
        builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<EIncidentStatus>());

        builder.HasOne(x => x.RelatedRisk)
            .WithMany()
            .HasForeignKey(x => x.RelatedRiskId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}