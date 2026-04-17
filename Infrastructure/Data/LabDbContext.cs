using Lab.Api.Domain.Entities;
using Lab.Api.Domain.Entities.Base;
using Lab.Api.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Lab.Api.Infrastructure.Data;

public class LabDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public Guid TenantId { get; }

    public LabDbContext(DbContextOptions<LabDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        TenantId = tenantProvider.TenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configure the TenantId for all entities.
                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("TenantId")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(Tenant))
                    .WithMany()
                    .HasForeignKey("TenantId")
                    .OnDelete(DeleteBehavior.Restrict);

                // Apply a global TenantId filter.
                var setTenantFilterMethod = typeof(LabDbContext)
                    .GetMethod(nameof(SetTenantFilter), (BindingFlags.NonPublic | BindingFlags.Instance))!
                    .MakeGenericMethod(entityType.ClrType);

                setTenantFilterMethod.Invoke(this, [modelBuilder]);
            }
        }

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CpfCnpj).IsRequired(false).HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired(false).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber).IsRequired(false).HasMaxLength(20);
        });

        modelBuilder.Entity<Customer>().HasIndex(e => e.CpfCnpj).IsUnique();
        modelBuilder.Entity<Customer>().HasIndex(e => e.Email).IsUnique();

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.HasOne(e => e.Customer)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Service)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.CreatedByUser)
               .WithMany()
               .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Offering>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).IsRequired(false).HasColumnType("decimal(18,2)");
        });
    }

    private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : TenantEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => e.TenantId == TenantId);
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Offering> Services { get; set; }
}


