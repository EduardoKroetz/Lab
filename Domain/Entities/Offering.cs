using Lab.Api.Domain.Entities.Base;

namespace Lab.Api.Domain.Entities;

public class Offering : TenantEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}
