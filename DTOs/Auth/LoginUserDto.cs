using System.ComponentModel.DataAnnotations;

namespace Lab.Api.DTOs.Auth;

public class LoginUserDto
{
    [Required(ErrorMessage = "Informe o email.")]
    [EmailAddress(ErrorMessage = "Informe um email válido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha.")]
    [MinLength(6, ErrorMessage = "A senha deve conter no mínimo 6 caracteres.")]
    public string Password { get; set; }
}
