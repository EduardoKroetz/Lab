using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Tenants;

public class UpdateCurrentTenantRequest
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
    public string Name { get; set; }
}