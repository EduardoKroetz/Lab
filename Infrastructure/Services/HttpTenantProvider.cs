using Lab.Api.Common.Extensions;
using Lab.Api.Infrastructure.Services.Interfaces;

namespace Lab.Api.Infrastructure.Services;

public class HttpTenantProvider : ITenantProvider
{
    public Guid TenantId { get; }

    public HttpTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HTTP context is not available.");

        TenantId = httpContext.User.GetTenantId();
    }

}
