using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Risk : TenantEntity
{
    internal Risk() { } // EF
    public Risk(Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact)
    {
        Status = ERiskStatus.Identified;
        AssetId = assetId;
        ThreatId = threatId;
        VulnerabilityId = vulnerabilityId;

        ChangeProbability(probability);
        ChangeImpact(impact);
    }

    public Guid AssetId { get; private set; }
    public Guid ThreatId { get; private set; }
    public Guid VulnerabilityId { get; private set; }

    public int Probability { get; private set; }
    public int Impact { get; private set; }

    public ERiskStatus Status { get; private set; }

    public ERiskTreatment? Treatment { get; private set; }
    public string? TreatmentDescription { get; private set; }

    public double Score => CalculateScore(Probability, Impact, EffectivenessOnProbability, EffectivenessOnImpact);

    public ERiskLevel Level => Score switch
    {
        >= 20 => ERiskLevel.Critical,
        >= 15 => ERiskLevel.High,
        >= 10 => ERiskLevel.Medium,
        _ => ERiskLevel.Low
    };

    public double EffectivenessOnProbability { get; private set; }
    public double EffectivenessOnImpact { get; private set; }

    #region Navigation Properties 

    public Asset Asset { get; private set; } = null!;
    public Threat Threat { get; private set; } = null!;
    public Vulnerability Vulnerability { get; private set; } = null!;

    #endregion

    private readonly List<RiskControl> _riskControls = [];
    public IReadOnlyCollection<RiskControl> RiskControls => _riskControls.AsReadOnly();

    public void AddControl(Guid controlId, EControlType controlType, int effectiveness)
    {
        if (_riskControls.Any(rc => rc.ControlId == controlId))
            throw new DomainException("Controle já está vinculado ao risco");

        var riskControl = new RiskControl(Id, controlId, controlType, effectiveness);
        _riskControls.Add(riskControl);
        RecalculateEffectiveness();
    }

    public void RemoveControl(Guid controlId)
    {
        var riskControl = _riskControls.FirstOrDefault(rc => rc.ControlId == controlId)
            ?? throw new DomainException("Controle não encontrado nesse risco");

        _riskControls.Remove(riskControl);
        RecalculateEffectiveness();
    }

    public void RecalculateEffectiveness()
    {
        var probabilityRiskControls = _riskControls.Where(r => r.ControlType is EControlType.Preventive).ToList();
        var impactRiskControls = _riskControls.Where(r => r.ControlType is EControlType.Detective or EControlType.Corrective).ToList();

        EffectivenessOnProbability = CalculateEffectiveness(probabilityRiskControls);
        EffectivenessOnImpact = CalculateEffectiveness(impactRiskControls);
    }

    private static double CalculateScore(int probability, int impact, double effProbability, double effImpact)
    {
        var p = probability * (1 - effProbability / 100.0);
        var i = impact * (1 - effImpact / 100.0);
        return p * i;
    }

    private static double CalculateEffectiveness(List<RiskControl> controls)
    {
        if (!controls.Any())
            return 0;

        var combined = 1 - controls.Aggregate(1.0, (product, rc) => product * (1 - rc.Effectiveness / 100.0));
        return combined * 100;
    }

    public void ChangeProbability(int probability)
    {
        if (probability <= 0 || probability > 5)
            throw new DomainException("A probabilidade deve estar entre 0 e 5");

        Probability = probability;

        RecalculateEffectiveness();
    }

    public void ChangeImpact(int impact)
    {
        if (impact <= 0 || impact > 5)
            throw new DomainException("O impacto deve estar entre 0 e 5");

        Impact = impact;

        RecalculateEffectiveness();
    }

    public void ChangeStatus(ERiskStatus newStatus)
    {
        if (Status != ERiskStatus.Identified && newStatus == ERiskStatus.Identified)
            throw new DomainException("O status não pode voltar para identificado.");

        Status = newStatus;
    }

    public void ChangeTreatment(ERiskTreatment? newTreatment, string? description = null)
    {
        if (newTreatment == ERiskTreatment.Mitigate && !_riskControls.Any())
            throw new DomainException("Para Mitigar, o risco deve possuir ao menos um controle vinculado.");

        if (newTreatment is ERiskTreatment.Accept or ERiskTreatment.Transfer && string.IsNullOrWhiteSpace(description))
            throw new DomainException("Tratamento requer justificativa/descrição.");

        Treatment = newTreatment;
        TreatmentDescription = description;
    }
}
