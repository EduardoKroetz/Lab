using Lab.Domain.Common;
using Lab.Domain.Common.Models;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class Risk : TenantEntity
{
    internal Risk() { } // EF
    private Risk(Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact)
    {
        Status = ERiskStatus.Identified;
        AssetId = assetId;
        ThreatId = threatId;
        VulnerabilityId = vulnerabilityId;
        Probability = probability;
        Impact = impact;
    }

    public Guid AssetId { get; private set; }
    public Guid ThreatId { get; private set; }
    public Guid VulnerabilityId { get; private set; }

    public int Probability { get; private set; }
    public int Impact { get; private set; }

    public ERiskStatus Status { get; private set; }

    public ERiskTreatmentStrategy TreatmentStrategy { get; private set; }
    public string? TreatmentDescription { get; private set; }

    public Asset Asset { get; private set; }
    public Threat Threat { get; private set; }
    public Vulnerability Vulnerability { get; private set; }

    public int Score => Probability * Impact;
    public ERiskLevel Level => Score switch
    {
        >= 20 => ERiskLevel.Critical,
        >= 15 => ERiskLevel.High,
        >= 10 => ERiskLevel.Medium,
        _ => ERiskLevel.Low
    };

    public List<RiskControl> RiskControls { get; private set; }

    public static Result<Risk> Create(Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact)
    {
        var result = ValidateValues(probability, impact);
        if (!result.Succeeded)
            return Result<Risk>.Failure(result.Errors);

        var newRisk = new Risk(assetId, threatId, vulnerabilityId, probability, impact);

        return Result<Risk>.Success(newRisk);
    }


    public Result Update(Guid assetId, Guid threatId, Guid vulnerabilityId, int probability, int impact)
    {
        var result = ValidateValues(probability, impact);
        if (!result.Succeeded)
            return result;

        AssetId = assetId;
        ThreatId = threatId;
        VulnerabilityId = vulnerabilityId;
        Probability = probability;
        Impact = impact;

        return Result.Success();
    }


    private static Result ValidateValues(int probability, int impact)
    {
        var errors = new List<string>();

        if (probability <= 0 || probability > 5)
            errors.Add("A probabilidade deve estar entre 0 e 5");

        if (impact <= 0 || impact > 5)
            errors.Add("O impacto deve estar entre 0 e 5");

        if (errors.Any())
            return Result.Failure(errors);

        return Result.Success();
    }

    public Result ChangeStatus(ERiskStatus newStatus)
    {
        if (Status != ERiskStatus.Identified && newStatus == ERiskStatus.Identified)
            return Result.Failure("O status não pode voltar para identificado.");

        return Result.Success();
    }
}
