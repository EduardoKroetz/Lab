using Lab.Api.Common.Security;
using System.Security;
using System.Security.Claims;

namespace Lab.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetTenantId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(AppClaimTypes.TenantId);

        if (string.IsNullOrEmpty(value))
            throw new SecurityException("TenantId claim is missing.");

        if (!int.TryParse(value, out var tenantId))
            throw new SecurityException("TenantId claim is invalid.");

        return tenantId;
    }
}
