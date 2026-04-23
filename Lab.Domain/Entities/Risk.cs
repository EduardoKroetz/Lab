using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;


public class Risk : TenantEntity
{
    internal Risk() { } // EF
    public Risk(Asset asset, Threat threat, Vulnerability vulnerability, int probability, int impact, ERiskLevel level, ERiskStatus status)
    {
        Status = status;
        Asset = asset;
        Threat = threat;
        Vulnerability = vulnerability;
        Probability = probability;
        Impact = impact;
        Level = level;

        Validate();
    }

    public Guid AssetId { get; private set; }
    public Guid ThreatId { get; private set; }
    public Guid VulnerabilityId { get; private set; }

    public int Probability { get; private set; }
    public int Impact { get; private set; }

    public ERiskLevel Level { get; private set; }
    public ERiskStatus Status { get; private set; }

    public Asset Asset { get; private set; }
    public Threat Threat { get; private set; }
    public Vulnerability Vulnerability { get; private set; }

    public int Score => Probability * Impact;

    public void Update(Asset asset, Threat threat, Vulnerability vulnerability, int probability, int impact, ERiskLevel level, ERiskStatus status)
    {
        Status = status;
        Asset = asset;
        Threat = threat;
        Vulnerability = vulnerability;
        Probability = probability;
        Impact = impact;
        Level = level;

        Validate();
    }

    private void Validate()
    {
        if (Probability <= 0 || Probability > 5)
            throw new InvalidOperationException("A probabilidade deve estar entre 0 e 5");

        if (Impact <= 0 || Impact > 5)
            throw new InvalidOperationException("O impacto deve estar entre 0 e 5");
    }
}
