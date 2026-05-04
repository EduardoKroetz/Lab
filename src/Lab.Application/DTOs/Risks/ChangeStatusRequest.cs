using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Risks;

public class ChangeStatusRequest
{
    public Guid RiskId { get; set; }
    public ERiskStatus Status { get; set; }
    public string? Description { get; set; }
    public bool CloseManually { get; set; }
}
