using Lab.Domain.Common;

namespace Lab.Domain.Entities;

public class Customer : TenantEntity
{
    protected Customer() { } // EF
    public Customer(string name, string? cpfCnpj, string? email, string? phoneNumber)
    {
        Name = name;
        CpfCnpj = cpfCnpj;
        Email = email;
        PhoneNumber = phoneNumber;

        Validate();
    }

    public string Name { get; private set; }
    public string? CpfCnpj { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }

    public void Update(string name, string? cpfCnpj, string? email, string? phoneNumber)
    {
        Name = name;
        CpfCnpj = cpfCnpj;
        Email = email;
        PhoneNumber = phoneNumber;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome é obrigatório.");
    }
}
