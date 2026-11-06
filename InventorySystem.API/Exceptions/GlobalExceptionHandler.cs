using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace InventorySystem.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) 
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occured: {Message}", exception.Message);

            var problemDetails = exception switch
            {
                NotFoundException notFound => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.NotFound,
                    Title = "Resource not found",
                    Detail = notFound.Message,
                    Instance = httpContext.Request.Path
                },
                BusinessRuleException businessRuleException => new ProblemDetails 
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Business rule violation",
                    Detail = businessRuleException.Message,
                    Instance = httpContext.Request.Path
                },
                ArgumentException argEx => new ProblemDetails 
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Title = "Invalid argument",
                    Detail = argEx.Message,
                    Instance = httpContext.Request.Path
                },
                _ => new ProblemDetails
                {
                    Status = (int)HttpStatusCode.InternalServerError,
                    Title = "Server Error",
                    Detail = "An unexpected error ocurred. Please contact support.",
                    Instance = httpContext.Request.Path
                }
            };

            httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
