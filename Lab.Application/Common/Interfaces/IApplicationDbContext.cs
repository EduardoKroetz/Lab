using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lab.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Appointment> Appointments { get; set; }
    DbSet<Offering> Offerings { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
