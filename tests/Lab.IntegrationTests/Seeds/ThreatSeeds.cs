using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Infrastructure.Data;

namespace Lab.IntegrationTests.Seeds;

public static class ThreatSeeds
{

    public static async Task<Threat> SeedThreatAsync(ApplicationDbContext dbContext)
    {
        var threat = new Threat("Server", "desc", EThreatCategory.HumanError);

        dbContext.Threats.Add(threat);
        await dbContext.SaveChangesAsync();

        return threat;
    }
}
