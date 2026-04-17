using System.ComponentModel.DataAnnotations;

namespace Lab.Api.Application.DTOs.Offerings;

public class UpsertOfferingDto
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
    public string? Description { get; set; }

    public decimal? Price { get; set; }
}
