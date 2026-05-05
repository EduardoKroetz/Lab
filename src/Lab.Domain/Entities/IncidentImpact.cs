using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class IncidentImpact : TenantEntity
{
    internal IncidentImpact() { } // EF
    public IncidentImpact(Guid incidentId, EIncidentImpactType type, int severityScore, string? description)
    {
        IncidentId = incidentId;

        SetType(type);
        SetSeverityScore(severityScore);
        SetDescription(description);
    }

    public Guid IncidentId { get; private set; }
    public EIncidentImpactType Type { get; private set; }
    public int SeverityScore { get; private set; }
    public EIncidentImpactSeverityLevel SeverityLevel => SeverityScore switch
    {
        >= 9 => EIncidentImpactSeverityLevel.Critical,
        >= 6 => EIncidentImpactSeverityLevel.High,
        >= 3 => EIncidentImpactSeverityLevel.Medium,
        _ => EIncidentImpactSeverityLevel.Low
    };

    public string? Description { get; private set; } = null!;

    public Incident Incident { get; private set; } = null!;

    public void SetType(EIncidentImpactType type)
    {
        Type = type;
    }

    public void SetSeverityScore(int severityScore)
    {
        if (severityScore < 0 || severityScore > 10)
            throw new DomainException("A severidade do impacto deve estar entre 0 e 10.");

        SeverityScore = severityScore;
    }

    public void SetDescription(string? description)
    {
        if (description != null && description.Length > 500)
            throw new DomainException("A descrição do impacto não pode exceder 500 caracteres.");

        Description = description;
    }

}
