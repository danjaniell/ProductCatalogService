using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalogService.Middleware;

/// <summary>
/// Global exception handling middleware to catch unhandled exceptions and return standardized error responses.
/// </summary>
public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="httpContext">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await next(httpContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    /// <summary>
    /// Handles the exception and writes an error response to the HTTP context.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = "Please try again later. If the issue persists, contact support.",
            Type = "https://tools.ietf.org/html/rfc7807#section-3.1" // Common type for error details
        };

        // For development, include more details. In production, be less verbose.
        if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
        {
            problemDetails.Detail = exception.Message;
            problemDetails.Extensions["traceId"] = context.TraceIdentifier;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        var json = JsonSerializer.Serialize(problemDetails);
        return context.Response.WriteAsync(json);
    }
}
