using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class IncidentImpact : TenantEntity
{
    internal IncidentImpact() { } // EF
    public IncidentImpact(Incident incident, EIncidentImpactType type, EIncidentImpactLevel level)
    {
        Incident = incident;
        IncidentId = incident.Id;
        Type = type;
        Level = level;
    }

    public Guid? IncidentId { get; private set; }
    public EIncidentImpactType Type { get; private set; }
    public EIncidentImpactLevel Level { get; private set; }

    public Incident Incident { get; private set; }

    public void Update(Incident incident, EIncidentImpactType type, EIncidentImpactLevel level)
    {
        Incident = incident;
        IncidentId = incident.Id;
        Type = type;
        Level = level;
    }
}
