using Lab.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Incidents;

public class UpsertIncidentRequest
{
    [Required(ErrorMessage = "Informe a descrição.")]
    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Informe a data da ocorrência.")]
    public DateTime DateOccurred { get; set; }

    public Guid RiskId { get; set; }

    [Required(ErrorMessage = "Informe o status.")]
    public EIncidentStatus Status { get; set; }
}
