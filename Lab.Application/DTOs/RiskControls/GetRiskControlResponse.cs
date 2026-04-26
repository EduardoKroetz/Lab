using Lab.Domain.Enums;

namespace Lab.Application.DTOs.RiskControls;

public class GetRiskControlResponse
{
    public Guid Id { get; set; }
    public Guid RiskId { get; set; }
    public Guid ControlId { get; set; }
    public string ControlName { get; set; }
    public EControlType ControlType { get; set; }
    public int Effectiveness { get; set; }
}
