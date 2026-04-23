using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.Mappings;

public class IncidentImpactMapping : IEntityTypeConfiguration<IncidentImpact>
{
    public void Configure(EntityTypeBuilder<IncidentImpact> builder)
    {
        builder.HasOne(x => x.Incident)
            .WithMany()
            .HasForeignKey(x => x.IncidentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<EIncidentImpactType>());
        builder.Property(x => x.Level).HasConversion(new EnumToStringConverter<EIncidentImpactLevel>());
    }
}