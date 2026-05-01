namespace Lab.Domain.Common;

public class TenantEntity : BaseEntity, ITenantEntity
{
    public Guid TenantId { get; set; }
}

public interface ITenantEntity
{
    Guid TenantId { get; set; }
}