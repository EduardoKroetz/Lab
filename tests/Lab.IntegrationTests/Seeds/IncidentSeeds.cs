using Lab.Domain.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Infrastructure.Data;

namespace Lab.IntegrationTests.Seeds;

public static class IncidentSeeds
{

    public static async Task<Incident> SeedIncidentAsync(ApplicationDbContext dbContext, ISystemClock clock)
    {
        var risk = await RiskSeeds.SeedRiskAsync(dbContext, clock);
        var incident = new Incident("An attempt to access a protected endpoint without authentication was blocked in the logs.", DateTime.UtcNow.AddDays(-3), EIncidentStatus.Investigating, risk.Id);

        dbContext.Incidents.Add(incident);
        await dbContext.SaveChangesAsync();

        return incident;
    }
}