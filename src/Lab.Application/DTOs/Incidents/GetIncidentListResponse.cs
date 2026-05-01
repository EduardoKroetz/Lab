using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Incidents;

public class GetIncidentListResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public DateTime DateOccurred { get; set; }
    public Guid? RelatedRiskId { get; set; }
    public EIncidentStatus Status { get; set; }
}
