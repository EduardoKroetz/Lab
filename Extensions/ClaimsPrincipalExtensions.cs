using System.Security.Claims;

namespace Lab.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out Guid parsedUserId))
            throw new UnauthorizedAccessException("Invalid user ID claim.");

        return parsedUserId;
    }

}
