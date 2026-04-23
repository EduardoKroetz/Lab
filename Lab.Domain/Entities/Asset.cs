using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class Asset : TenantEntity
{
    internal Asset() { } // EF
    public Asset(string name, string description, EAssetType type, EAssetCriticality criticality)
    {
        Name = name;
        Description = description;
        Type = type;
        Criticality = criticality;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public EAssetType Type { get; private set; }
    public EAssetCriticality Criticality { get; private set; }

    public void Update(string name, string description, EAssetType type, EAssetCriticality criticality)
    {
        Name = name;
        Description = description;
        Type = type;
        Criticality = criticality;
    }
}
