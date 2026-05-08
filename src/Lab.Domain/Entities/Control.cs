using Lab.Domain.Common;
using Lab.Domain.Enums;

namespace Lab.Domain.Entities;

public class Control : TenantEntity
{
    internal Control() { } // EF
    public Control(string name, string description, EControlType type, EControlCategory category)
    {
        Name = name;
        Description = description;
        Type = type;
        Category = category;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public EControlType Type { get; private set; }
    public EControlCategory Category { get; private set; }

    public List<RiskControl> RiskControls { get; set; }

    public void Update(string name, string description, EControlType type, EControlCategory category)
    {
        Name = name;
        Description = description;
        Type = type;
        Category = category;
    }
}
