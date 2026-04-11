using Microsoft.AspNetCore.Identity;

namespace Lab.Api.Common;

public class PtBrIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
        => new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"A senha deve ter no mínimo {length} caracteres."
        };

    public override IdentityError PasswordRequiresDigit()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "A senha deve conter pelo menos um número."
        };

    public override IdentityError PasswordRequiresUpper()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "A senha deve conter pelo menos uma letra maiúscula."
        };

    public override IdentityError PasswordRequiresLower()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "A senha deve conter pelo menos uma letra minúscula."
        };

    public override IdentityError PasswordRequiresNonAlphanumeric()
        => new IdentityError
        {
            Code = nameof(PasswordRequiresNonAlphanumeric),
            Description = "A senha deve conter pelo menos um caractere especial."
        };
}