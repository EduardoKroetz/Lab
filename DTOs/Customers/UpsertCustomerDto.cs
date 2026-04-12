using Lab.Api.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Lab.Api.DTOs.Customers;

public class UpsertCustomerDto
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Name { get; set; }

    [CpfCnpj(ErrorMessage = "Informe um CPF ou CNPJ válido.")]
    [MaxLength(20, ErrorMessage = "O CPF ou CNPJ deve ter no máximo 20 caracteres.")]
    public string? CpfCnpj { get; set; }

    [MaxLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string? Email { get; set; }

    [MaxLength(20, ErrorMessage = "O número de telefone deve ter no máximo 20 caracteres.")]
    public string? PhoneNumber { get; set; }
}
