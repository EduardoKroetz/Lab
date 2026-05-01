using Lab.Application.Common.Interfaces;
using Lab.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Lab.Infrastructure.Services;

public class HttpTenantProvider : ITenantProvider
{
    public Guid TenantId { get; }

    public HttpTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HTTP context is not available.");

        var isAuthenticated = httpContext.User?.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
            return;

        TenantId = httpContext.User.GetTenantId();
    }

}
