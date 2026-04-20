using Lab.Api.Application.DTOs;
using Lab.Api.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace Lab.Api.Filters;

public class GlobalExceptionHandler : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, errorMessage) = exception switch
        {
            NotFoundException notFoundException => (StatusCodes.Status404NotFound, notFoundException.Message),
            BadRequestException badRequestException => (StatusCodes.Status400BadRequest, badRequestException.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Acesso não autorizado"),
            _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado.")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        httpContext.Response.WriteAsJsonAsync(new ResponseDto(errorMessage), cancellationToken);

        return new ValueTask<bool>(true);
    }
}
