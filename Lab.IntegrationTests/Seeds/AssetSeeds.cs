using Lab.Domain.Entities;
using Lab.Domain.Enums;
using Lab.Infrastructure.Data;

namespace Lab.IntegrationTests.Seeds;

public static class AssetSeeds
{

    public static async Task<Asset> SeedAssetAsync(ApplicationDbContext dbContext)
    {
        var asset = new Asset("Server", "desc", EAssetType.Infrastructure, EAssetCriticality.High);

        dbContext.Assets.Add(asset);
        await dbContext.SaveChangesAsync();

        return asset;
    }
}
