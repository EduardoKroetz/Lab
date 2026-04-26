using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Asset : TenantEntity
{
    internal Asset() { } // EF
    public Asset(string name, string description, EAssetType type, EAssetCriticality criticality)
    {
        Validate(type, criticality);

        Name = name;
        Description = description;
        Type = type;
        Criticality = criticality;
    }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public EAssetType Type { get; private set; }
    public EAssetCriticality Criticality { get; private set; }
    public bool Enabled { get; private set; }

    public List<Risk> Risks { get; private set; } = null!;

    public void Update(string name, string description, EAssetType type, EAssetCriticality criticality)
    {
        Validate(type, criticality);

        Name = name;
        Description = description;
        Type = type;
        Criticality = criticality;
    }

    private static void Validate(EAssetType type, EAssetCriticality criticality)
    {
        if (!Enum.IsDefined(type))
            throw new DomainException("Criticidade inválida");

        if (!Enum.IsDefined(criticality))
            throw new DomainException("Criticidade inválida");
    }
}
