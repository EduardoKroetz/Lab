using Lab.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Risks;

public class UpdateRiskRequest
{
    [Range(1, 5, ErrorMessage = "A probabilidade deve estar entre 1 e 5.")]
    public int Probability { get; set; }

    [Range(1, 5, ErrorMessage = "O impacto deve estar entre 1 e 5.")]
    public int Impact { get; set; }

    [Required(ErrorMessage = "Informe o status.")]
    public ERiskStatus Status { get; set; }

    public ERiskTreatment? Treatment { get; set; }
    public string? TreatmentDescription { get; set; }
}
