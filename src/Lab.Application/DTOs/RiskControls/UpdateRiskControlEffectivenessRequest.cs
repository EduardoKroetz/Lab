using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.RiskControls;

public class UpdateRiskControlEffectivenessRequest
{
    [Required(ErrorMessage = "Informe o ID do controle.")]
    public Guid ControlId { get; set; }

    [Range(0, 100, ErrorMessage = "A eficácia deve estar entre 0 e 100.")]
    public int Effectiveness { get; set; }
}
