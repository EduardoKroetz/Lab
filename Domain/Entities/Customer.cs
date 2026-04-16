using Lab.Api.Domain.Entities.Base;

namespace Lab.Api.Domain.Entities;

public class Customer : TenantEntity
{
    public string Name { get; set; }
    public string? CpfCnpj { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}

