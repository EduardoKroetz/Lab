using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Risks;

public class GetRiskListResponse
{
    public Guid Id { get; set; }
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = null!;
    public Guid ThreatId { get; set; }
    public string ThreatName { get; set; } = null!;
    public Guid VulnerabilityId { get; set; }
    public string VulnerabilityName { get; set; } = null!;
    public int Probability { get; set; }
    public int Impact { get; set; }
    public double Score { get; set; }
    public double EffectivenessOnProbability { get; set; }
    public double EffectivenessOnImpact { get; set; }
    public ERiskLevel Level { get; set; }
    public ERiskStatus Status { get; set; }
}
