using Lab.Api.Entities;
using Lab.Api.Entities.Base;
using Lab.Api.Providers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Lab.Api.Data;

public class LabDbContext : DbContext
{
    public int TenantId { get; }

    public LabDbContext(DbContextOptions<LabDbContext> options, ITenantProvider tenantProvider) : base(options)
    {
        TenantId = tenantProvider.TenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {    
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configure the TenantId for all entities.
                modelBuilder.Entity(entityType.ClrType)
                    .Property<int>("TenantId")
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
    }

    private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : TenantEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => e.TenantId == TenantId);
    }

    public DbSet<Tenant> Tenants { get; set; }

}


