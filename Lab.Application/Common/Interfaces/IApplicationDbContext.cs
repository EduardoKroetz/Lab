using Lab.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lab.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; set; }

    DbSet<Asset> Assets { get; set; }
    DbSet<Threat> Threats { get; set; }
    DbSet<Vulnerability> Vulnerabilities { get; set; }
    DbSet<Risk> Risks { get; set; }
    DbSet<Control> Controls { get; set; }
    DbSet<RiskControl> RiskControls { get; set; }
    DbSet<Incident> Incidents { get; set; }
    DbSet<IncidentImpact> IncidentImpacts { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}
