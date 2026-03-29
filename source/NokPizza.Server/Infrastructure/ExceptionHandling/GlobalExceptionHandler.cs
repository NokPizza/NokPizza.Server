using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace NokPizza.Server.Infrastructure.ExceptionHandling;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        const int statusCode = StatusCodes.Status500InternalServerError;

        logger.LogError(exception, "Unhandled exception while processing request.");

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Server error",
            Detail = "An unexpected error occurred.",
            Type = $"https://httpstatuses.com/{statusCode}",
        };
        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}
