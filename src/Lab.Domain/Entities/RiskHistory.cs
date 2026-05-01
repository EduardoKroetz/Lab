using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class RiskHistory : TenantEntity
{
    protected RiskHistory() { } // EF
    public RiskHistory(Guid riskId, ERiskHistoryEvent historyEvent, Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact, ERiskStatus status, ERiskTreatment? treatment, string? treatmentDescription, int rawScore, double residualScore, ERiskLevel level, double effectivenessOnProbability, double effectivenessOnImpact)
    {
        RiskId = riskId;
        Event = historyEvent;
        Snapshot = new RiskSnapshot
        {
            Id = riskId,
            AssetId = assetId,
            ThreatId = threatId,
            VulnerabilityId = vulnerabilityId,
            Probability = probability,
            Impact = impact,
            Status = status,
            Treatment = treatment,
            TreatmentDescription = treatmentDescription,
            RawScore = rawScore,
            ResidualScore = residualScore,
            Level = level,
            EffectivenessOnProbability = effectivenessOnProbability,
            EffectivenessOnImpact = effectivenessOnImpact
        };
        CreatedAt = DateTime.Now;
    }

    public ERiskHistoryEvent Event { get; set; }
    public Guid RiskId { get; set; }
    public RiskSnapshot Snapshot { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RiskSnapshot
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public Guid ThreatId { get; set; }
    public Guid VulnerabilityId { get; set; }

    public int Probability { get; set; }
    public int Impact { get; set; }

    public ERiskStatus Status { get; set; }

    public ERiskTreatment? Treatment { get; set; }
    public string? TreatmentDescription { get; set; }

    public int RawScore { get; set; }
    public double ResidualScore { get; set; }
    public ERiskLevel Level { get; set; }

    public double EffectivenessOnProbability { get; set; }
    public double EffectivenessOnImpact { get; set; }
}