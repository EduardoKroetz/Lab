using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Risks;

public class ChangeTreatmentRequest
{
    public Guid RiskId { get; set; }
    public ERiskTreatment Treatment { get; set; }
    public string? Description { get; set; }
}