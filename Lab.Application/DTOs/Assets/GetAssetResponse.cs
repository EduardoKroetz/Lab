using Lab.Domain.Enums;

namespace Lab.Application.DTOs.Assets;

public class GetAssetResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public EAssetType Type { get; set; }
    public EAssetCriticality Criticality { get; set; }
}
