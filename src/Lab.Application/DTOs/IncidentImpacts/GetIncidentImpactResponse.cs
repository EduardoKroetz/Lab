using Lab.Domain.Enums;

namespace Lab.Application.DTOs.IncidentImpacts;

public class GetIncidentImpactResponse
{
    public Guid Id { get; set; }
    public Guid? IncidentId { get; set; }
    public EIncidentImpactType Type { get; set; }
    public double SeverityScore { get; set; }
    public string? Description { get; set; }
    public EIncidentImpactSeverityLevel SeverityLevel { get; set; }
}
