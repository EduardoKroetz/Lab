using Lab.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.IncidentImpacts;

public class UpsertIncidentImpactRequest
{
    [Required(ErrorMessage = "Informe o tipo do impacto.")]
    public EIncidentImpactType Type { get; set; }

    [Range(1, 10, ErrorMessage = "A pontuação de severidade deve estar entre 1 e 10.")]
    public int SeverityScore { get; set; }

    [MaxLength(500, ErrorMessage = "A descrição do impacto não pode exceder {1} caracteres.")]
    public string? Description { get; set; }
}
