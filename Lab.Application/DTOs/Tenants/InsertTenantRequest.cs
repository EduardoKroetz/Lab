using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Tenants;

public class InsertTenantRequest
{
    [Required(ErrorMessage = "Informe o nome.")]
    [MaxLength(100, ErrorMessage = "O nome não pode exceder {1} caracteres.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Informe o usuário inicial do tenant")]
    public InsertTenantUserRequest User { get; set; }
}

public class InsertTenantUserRequest
{
    [Required(ErrorMessage = "Informe o email.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(6, ErrorMessage = "A senha deve conter no mínimo {1} caracteres.")]
    public string Password { get; set; }
}