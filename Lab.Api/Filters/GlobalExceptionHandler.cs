using Lab.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Filters;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = new ProblemDetails();

        switch (exception)
        {
            case NotFoundException:
            problem.Status = StatusCodes.Status404NotFound;
            problem.Detail = exception.Message;
            break;

            case UnauthorizedAccessException:
            problem.Status = StatusCodes.Status401Unauthorized;
            problem.Detail = "Acesso não autorizado";
            break;

            case ValidationException validationException:
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Detail = "Um ou mais erros de validação ocorreram.";

            if (validationException.Errors != null)
            {
                problem.Extensions["errors"] = validationException.Errors
                    .Select(e => new
                    {
                        field = e.Code,   // ou "name", "email", etc.
                        message = e.Message
                    });
            }
            break;

            case DomainException:
            problem.Status = StatusCodes.Status400BadRequest;
            problem.Detail = exception.Message;
            break;

            default:
            problem.Status = StatusCodes.Status500InternalServerError;
            problem.Detail = "Ocorreu um erro inesperado.";
            break;
        }

        httpContext.Response.StatusCode = problem.Status.Value;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
