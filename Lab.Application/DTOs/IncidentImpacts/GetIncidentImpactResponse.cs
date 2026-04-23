using Lab.Domain.Enums;

namespace Lab.Application.DTOs.IncidentImpacts;

public class GetIncidentImpactResponse
{
    public Guid Id { get; set; }
    public Guid? IncidentId { get; set; }
    public string IncidentDescription { get; set; } = null!;
    public EIncidentImpactType Type { get; set; }
    public EIncidentImpactLevel Level { get; set; }
}
