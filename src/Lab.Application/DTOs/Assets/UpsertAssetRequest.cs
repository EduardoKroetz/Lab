using Lab.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Assets;

public class UpsertAssetRequest
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Informe a descrição.")]
    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Informe o tipo.")]
    public EAssetType Type { get; set; }

    [Required(ErrorMessage = "Informe a criticidade.")]
    public EAssetCriticality Criticality { get; set; }
}
