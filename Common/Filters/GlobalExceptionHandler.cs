using Lab.Api.DTOs;
using Microsoft.AspNetCore.Diagnostics;

namespace Lab.Api.Common.Filters;

public class GlobalExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, errorMessage) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado"),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado.")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.WriteAsJsonAsync(new ResponseDto(errorMessage), cancellationToken);

        return new ValueTask<bool>(true);     
    }
}
