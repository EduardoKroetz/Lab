using Lab.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Filters;

public class GlobalExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, errorMessage) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado"),
            DomainException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado.")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var payload = new ProblemDetails
        {
            Detail = errorMessage,
            Status = statusCode
        };

        httpContext.Response.WriteAsJsonAsync(payload, cancellationToken);

        return new ValueTask<bool>(true);
    }
}
