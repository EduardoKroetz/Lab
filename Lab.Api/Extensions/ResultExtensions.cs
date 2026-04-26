using Lab.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.Succeeded)
            return new OkObjectResult(result.Value);

        return new BadRequestObjectResult(new ProblemDetails { Detail = result.Errors.FirstOrDefault() });
    }

    public static IActionResult ToActionResult(this Result result)
    {
        if (result.Succeeded)
            return new NoContentResult();

        return new BadRequestObjectResult(new ProblemDetails { Detail = result.Errors.FirstOrDefault() });
    }


}
