using Lab.Domain.Utils;
using System.ComponentModel.DataAnnotations;

namespace Lab.Application.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class CpfCnpjAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
            return true;

        bool valido = CpfCnpjUtils.IsCpfCnpj(value.ToString()!);
        return valido;
    }
}
