using Lab.Domain.Common;
using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Infrastructure.Data;

namespace Lab.IntegrationTests.Seeds;

public static class RiskSeeds
{
    public static async Task<Risk> SeedRiskAsync(ApplicationDbContext dbContext, Guid assetId, Guid threatId, Guid vulnerabilityId, ISystemClock clock)
    {
        var risk = new Risk(assetId, threatId, vulnerabilityId, probability: 3, impact: 4, reviewFixedDate: DateTime.UtcNow.AddMonths(2), null, clock);
        dbContext.Risks.Add(risk);

        await dbContext.SaveChangesAsync();

        return risk;
    }

    public static async Task<Risk> SeedRiskAsync(ApplicationDbContext dbContext, ISystemClock clock)
    {
        var (asset, threat, vulnerability) = await SeedBaseEntitiesAsync(dbContext);

        var risk = new Risk(asset.Id, threat.Id, vulnerability.Id, probability: 3, impact: 4, reviewFixedDate: DateTime.UtcNow.AddMonths(2), null, clock);
        dbContext.Risks.Add(risk);

        await dbContext.SaveChangesAsync();

        return risk;
    }

    public static async Task<(Asset, Threat, Vulnerability)> SeedBaseEntitiesAsync(ApplicationDbContext dbContext)
    {
        var asset = new Asset("Server", "desc", EAssetType.Infrastructure, EAssetCriticality.High);
        var threat = new Threat("Crash", "desc", EThreatCategory.Network);
        var vulnerability = new Vulnerability("No monitoring", "desc", EVulnerabilityType.Network);

        dbContext.Assets.Add(asset);
        dbContext.Threats.Add(threat);
        dbContext.Vulnerabilities.Add(vulnerability);
        await dbContext.SaveChangesAsync();

        return (asset, threat, vulnerability);
    }

    public static async Task<Control> SeedControlAsync(ApplicationDbContext dbContext)
    {
        var control = new Control("Monitoring", "desc", EControlType.Detective, EControlCategory.Technical);
        dbContext.Controls.Add(control);
        await dbContext.SaveChangesAsync();
        return control;
    }

}
