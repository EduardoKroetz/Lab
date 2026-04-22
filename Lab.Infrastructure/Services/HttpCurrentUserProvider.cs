using Lab.Application.Common.Interfaces;
using Lab.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;

namespace Lab.Infrastructure.Services;

public class HttpUserProvider : IUserProvider
{
    public Guid UserId { get; }

    public HttpUserProvider(IHttpContextAccessor httpContextAccessor)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HTTP context is not available.");

        UserId = httpContext.User.GetUserId();
    }

}
