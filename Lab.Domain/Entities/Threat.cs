using Lab.Domain.Common;
using Lab.Domain.Enums;
using Lab.Domain.Exceptions;

namespace Lab.Domain.Entities;

public class Threat : TenantEntity
{
    internal Threat() { } // EF
    public Threat(string name, string description, EThreatCategory category)
    {
        Validate(category);

        Name = name;
        Description = description;
        Category = category;
    }

    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public EThreatCategory Category { get; private set; }

    public List<Risk> Risks { get; private set; } = null!;

    public void Update(string name, string description, EThreatCategory category)
    {
        Validate(category);

        Name = name;
        Description = description;
        Category = category;
    }

    private static void Validate(EThreatCategory category)
    {
        if (!Enum.IsDefined(category))
            throw new DomainException("Categoria inválida");
    }
}
