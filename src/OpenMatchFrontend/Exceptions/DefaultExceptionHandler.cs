using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpenMatchFrontend.Exceptions;

public class DefaultExceptionHandler(ILogger<DefaultExceptionHandler> logger) : IExceptionHandler
{
    private readonly ProblemDetails _problemDetails = new ProblemDetails
    {
        Status = StatusCodes.Status500InternalServerError,
        Title = "Internal Error"
    };
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unexpected error occurred {Message}", exception.Message);

        await httpContext.Response.WriteAsJsonAsync(_problemDetails, cancellationToken: cancellationToken);
        return true;
    }
}