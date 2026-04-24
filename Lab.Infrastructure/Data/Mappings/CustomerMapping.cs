using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lab.Infrastructure.Data.Mappings;

public class CustomerMapping : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
        builder.Property(e => e.CpfCnpj).IsRequired(false).HasMaxLength(20);
        builder.Property(e => e.Email).IsRequired(false).HasMaxLength(100);
        builder.Property(e => e.PhoneNumber).IsRequired(false).HasMaxLength(20);

        builder.HasIndex(e => e.CpfCnpj).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
    }
}
