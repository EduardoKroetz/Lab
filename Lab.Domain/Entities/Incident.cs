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

    public Risk? RelatedRisk { get; private set; } = null!;

    private readonly List<IncidentImpact> _incidentImpacts = [];
    public IReadOnlyCollection<IncidentImpact> IncidentImpacts => _incidentImpacts.AsReadOnly();

    public void Update(string description, DateTime dateOccurred, EIncidentStatus status, Guid riskId)
    {
        Description = description;
        DateOccurred = dateOccurred;
        Status = status;
        RiskId = riskId;
    }

    public void AddImpact(EIncidentImpactType type, EIncidentImpactLevel level)
    {
        if (_incidentImpacts.Any(x => x.Type == type && x.Level == level))
            throw new DomainException("Este incidente já possui um impacto com o mesmo tipo e nível.");

        var incidentImpact = new IncidentImpact(Id, type, level);
        _incidentImpacts.Add(incidentImpact);
    }

    public void RemoveImpact(Guid impactId)
    {
        var incidentImpact = _incidentImpacts.FirstOrDefault(ii => ii.Id == impactId)
            ?? throw new DomainException("Impacto de incidente nao encontrado.");

        _incidentImpacts.Remove(incidentImpact);
    }
}
