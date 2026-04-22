using Lab.Domain.Constants;
using System.Security;
using System.Security.Claims;

namespace Lab.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out Guid parsedUserId))
            throw new UnauthorizedAccessException("Invalid user ID claim.");

        return parsedUserId;
    }

    public static Guid GetTenantId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(AppClaimTypes.TenantId);

        if (string.IsNullOrEmpty(value))
            throw new SecurityException("TenantId claim is missing.");

        if (!Guid.TryParse(value, out var tenantId))
            throw new SecurityException("TenantId claim is invalid.");

        return tenantId;
    }

}
