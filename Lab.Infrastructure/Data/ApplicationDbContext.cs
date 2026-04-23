using Lab.Application.Common.Interfaces;
using Lab.Domain.Common;
using Lab.Domain.Entities;
using Lab.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Lab.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public Guid TenantId { get; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantProvider tenantProvider) : base(options)
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
                modelBuilder.Entity(entityType.ClrType)
                    .Property<Guid>("TenantId")
                    .IsRequired();

                modelBuilder.Entity(entityType.ClrType)
                    .HasOne(typeof(Tenant))
                    .WithMany()
                    .HasForeignKey("TenantId")
                    .OnDelete(DeleteBehavior.Restrict);

                var setTenantFilterMethod = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                setTenantFilterMethod.Invoke(this, [modelBuilder]);
            }
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private void SetTenantFilter<TEntity>(ModelBuilder modelBuilder) where TEntity : TenantEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasQueryFilter(e => e.TenantId == TenantId);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await Database.BeginTransactionAsync(cancellationToken);
    }

    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Offering> Offerings { get; set; }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<Threat> Threats { get; set; }
    public DbSet<Vulnerability> Vulnerabilities { get; set; }
    public DbSet<Risk> Risks { get; set; }
    public DbSet<Control> Controls { get; set; }
    public DbSet<RiskControl> RiskControls { get; set; }
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<IncidentImpact> IncidentImpacts { get; set; }
}
