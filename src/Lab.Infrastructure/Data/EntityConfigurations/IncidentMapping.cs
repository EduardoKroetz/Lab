using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.EntityConfigurations;

public class IncidentMapping : IEntityTypeConfiguration<Incident>
{
    public void Configure(EntityTypeBuilder<Incident> builder)
    {
        builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
        builder.Property(x => x.DateOccurred).IsRequired();
        builder.Property(x => x.Status).HasConversion(new EnumToStringConverter<EIncidentStatus>());
        builder.Property(x => x.Score).IsRequired();

        builder.HasOne(x => x.Risk)
            .WithMany(x => x.Incidents)
            .HasForeignKey(x => x.RiskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.IncidentImpacts)
            .WithOne(x => x.Incident)
            .HasForeignKey(x => x.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
