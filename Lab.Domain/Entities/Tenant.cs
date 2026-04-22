using Lab.Domain.Common;

namespace Lab.Domain.Entities;

public class Tenant : BaseEntity
{
    protected Tenant() { } // EF
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
