using Lab.Application.DTOs.IncidentImpacts;
using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Incidents;

public class GetIncidentDetailResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public DateTime DateOccurred { get; set; }
    public Guid RiskId { get; set; }
    public EIncidentStatus Status { get; set; }
    public List<GetIncidentImpactResponse> Impacts { get; set; } = [];
}
