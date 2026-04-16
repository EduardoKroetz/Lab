using Lab.Api.Domain.Entities.Base;

namespace Lab.Api.Domain.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; }
}
