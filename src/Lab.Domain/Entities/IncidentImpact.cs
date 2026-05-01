using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class IncidentImpact : TenantEntity
{
    internal IncidentImpact() { } // EF
    public IncidentImpact(Guid incidentId, EIncidentImpactType type, EIncidentImpactLevel level)
    {
        IncidentId = incidentId;
        Type = type;
        Level = level;
    }

    public Guid IncidentId { get; private set; }
    public EIncidentImpactType Type { get; private set; }
    public EIncidentImpactLevel Level { get; private set; }

    public Incident Incident { get; private set; } = null!;

    public void Update(EIncidentImpactType type, EIncidentImpactLevel level)
    {
        Type = type;
        Level = level;
    }
}
