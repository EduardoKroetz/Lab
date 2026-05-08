using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Incident : TenantEntity
{
    internal Incident() { } // EF
    public Incident(string description, DateTime dateOccurred, EIncidentStatus status, Guid riskId)
    {
        Description = description;
        DateOccurred = dateOccurred;
        Status = status;
        RiskId = riskId;
    }

    public string Description { get; private set; } = null!;
    public DateTime DateOccurred { get; private set; }
    public Guid RiskId { get; private set; }
    public EIncidentStatus Status { get; private set; }
    public int Score { get; private set; }
    public EIncidentSeverityLevel SeverityLevel => Score switch
    {
        >= 36 => EIncidentSeverityLevel.Critical,
        >= 21 => EIncidentSeverityLevel.High,
        >= 11 => EIncidentSeverityLevel.Medium,
        _ => EIncidentSeverityLevel.Low
    };

    public Risk Risk { get; private set; } = null!;

    private readonly List<IncidentImpact> _incidentImpacts = [];
    public IReadOnlyCollection<IncidentImpact> IncidentImpacts => _incidentImpacts.AsReadOnly();

    public void Update(string description, DateTime dateOccurred, EIncidentStatus status, Guid riskId)
    {
        Description = description;
        DateOccurred = dateOccurred;
        Status = status;
        RiskId = riskId;
    }

    public IncidentImpact AddImpact(EIncidentImpactType type, int severityScore, string? description)
    {
        var incidentImpact = new IncidentImpact(Id, type, severityScore, description);
        _incidentImpacts.Add(incidentImpact);

        RecalculateScore();

        return incidentImpact;
    }

    public IncidentImpact UpdateImpact(Guid impactId, EIncidentImpactType type, int severityScore, string? description)
    {
        var incidentImpact = _incidentImpacts.FirstOrDefault(ii => ii.Id == impactId)
            ?? throw new DomainException("Impacto de incidente nao encontrado.");

        incidentImpact.SetType(type);
        incidentImpact.SetSeverityScore(severityScore);
        incidentImpact.SetDescription(description);

        RecalculateScore();

        return incidentImpact;
    }

    public void RemoveImpact(Guid impactId)
    {
        var incidentImpact = _incidentImpacts.FirstOrDefault(ii => ii.Id == impactId)
            ?? throw new DomainException("Impacto de incidente nao encontrado.");

        _incidentImpacts.Remove(incidentImpact);

        RecalculateScore();
    }

    private void RecalculateScore()
    {
        Score = _incidentImpacts.Sum(ii => ii.SeverityScore);
    }
}
