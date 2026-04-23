using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lab.Infrastructure.Data.Mappings;

public class AssetMapping : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(256);
        builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<EAssetType>());
        builder.Property(x => x.Criticality).HasConversion(new EnumToStringConverter<EAssetCriticality>());
    }
}
