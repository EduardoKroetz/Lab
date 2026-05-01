using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Threats;

public class GetThreatResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public EThreatCategory Category { get; set; }
}
