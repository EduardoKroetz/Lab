namespace Lab.Application.DTOs.RiskControls;

public class GetRiskControlResponse
{
    public Guid Id { get; set; }
    public Guid RiskId { get; set; }
    public int RiskScore { get; set; }
    public Guid ControlId { get; set; }
    public string ControlName { get; set; } = null!;
    public int? Effectiveness { get; set; }
}
