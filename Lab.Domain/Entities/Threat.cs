using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class Threat : TenantEntity
{
    internal Threat() { } // EF
    public Threat(string name, string description, EThreatCategory category)
    {
        Name = name;
        Description = description;
        Category = category;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public EThreatCategory Category { get; private set; }

    public void Update(string name, string description, EThreatCategory category)
    {
        Name = name;
        Description = description;
        Category = category;
    }
}
