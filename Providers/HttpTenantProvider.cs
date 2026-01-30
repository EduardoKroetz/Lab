using Lab.Api.Extensions;
using Lab.Api.Providers.Interfaces;

namespace Lab.Api.Providers;

public sealed class HttpTenantProvider : ITenantProvider
{
    public int TenantId { get; }

    public HttpTenantProvider(IHttpContextAccessor acessor)
    {
        var httpContext = acessor.HttpContext ?? throw new InvalidOperationException("HttpContext not available");

        TenantId = httpContext.User.GetTenantId();
    }
}
