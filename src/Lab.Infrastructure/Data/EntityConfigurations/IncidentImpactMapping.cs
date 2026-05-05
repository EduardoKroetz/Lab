using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.EntityConfigurations;

public class IncidentImpactMapping : IEntityTypeConfiguration<IncidentImpact>
{
    public void Configure(EntityTypeBuilder<IncidentImpact> builder)
    {
        builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<EIncidentImpactType>());
    }
}
