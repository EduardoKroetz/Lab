using Lab.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lab.Api.Data;

public class LabDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public LabDbContext(DbContextOptions<LabDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).IsRequired(false).HasColumnType("decimal(18,2)");
        }); 
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Service> Services { get; set; }
}


