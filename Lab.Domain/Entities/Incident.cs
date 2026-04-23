using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class Incident : TenantEntity
{
    internal Incident() { } // EF
    public Incident(string description, DateTime dateOccurred, EIncidentStatus status, Risk? relatedRisk)
    {
        Description = description;
        DateOccurred = dateOccurred;
        Status = status;
        RelatedRisk = relatedRisk;
        RelatedRiskId = relatedRisk?.Id;
    }

    public string Description { get; private set; }
    public DateTime DateOccurred { get; private set; }
    public Guid? RelatedRiskId { get; private set; }
    public EIncidentStatus Status { get; private set; }

    public Risk? RelatedRisk { get; private set; }

    public void Update(string description, DateTime dateOccurred, EIncidentStatus status, Risk? relatedRisk)
    {
        Description = description;
        DateOccurred = dateOccurred;
        Status = status;
        RelatedRisk = relatedRisk;
        RelatedRiskId = relatedRisk?.Id;
    }
}
