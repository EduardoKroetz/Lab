using Lab.Api.Domain.Entities.Base;

namespace Lab.Api.Domain.Entities;

public class Tenant : BaseEntity
{
    public Tenant(string name)
    {
        Name = name;

        Validate();
    }

    public string Name { get; private set; }

    public void Update(string name)
    {
        Name = name;

        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Nome é obrigatório.");
    }
}
