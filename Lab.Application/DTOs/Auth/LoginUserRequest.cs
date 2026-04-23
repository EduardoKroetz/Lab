using System.ComponentModel.DataAnnotations;

namespace Lab.Application.DTOs.Auth;

public class LoginUserRequest
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(6, ErrorMessage = "A senha deve conter no mínimo {1} caracteres.")]
    public string Password { get; set; }
}
