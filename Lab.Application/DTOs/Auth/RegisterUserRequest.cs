using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Auth;

public class RegisterUserRequest
{
    [Required(ErrorMessage = "Informe o email.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(6, ErrorMessage = "A senha deve conter no mínimo {1} caracteres.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Informe o ID do tenant.")]
    public Guid TenantId { get; set; }
}
