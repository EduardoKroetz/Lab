using Lab.Application.Common.Interfaces;

namespace Lab.IntegrationTests.Common;

public class FakeTenantProvider : ITenantProvider
{
    public Guid TenantId { get; }

    public FakeTenantProvider(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
