using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Controls;

public class GetControlResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public EControlType Type { get; set; }
    public EControlCategory Category { get; set; }
}
