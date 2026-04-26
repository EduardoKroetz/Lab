using Lab.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.IncidentImpacts;

public class UpsertIncidentImpactRequest
{
    [Required(ErrorMessage = "Informe o tipo do impacto.")]
    public EIncidentImpactType Type { get; set; }

    [Required(ErrorMessage = "Informe o nivel do impacto.")]
    public EIncidentImpactLevel Level { get; set; }
}
