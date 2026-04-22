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
            entity.HasOne(e => e.Offering)
                .WithMany()
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(x => x.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Offering>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).IsRequired(false).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(entity => entity.Name).IsRequired().HasMaxLength(100);
        });
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
}
