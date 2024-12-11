using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostAPI.Handlers
{
    /// <summary>
    /// A global exception handler that logs unhandled exceptions and returns a standardized error response.
    /// </summary>
    /// <param name="logger">The logger used to log exception details.</param>
    public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        /// <summary>
        /// Attempts to handle an exception by logging it and returning a ProblemDetails response.
        /// </summary>
        /// <param name="httpContext">The HTTP context in which the exception occurred.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the exception was handled.</returns>
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            logger.LogError(exception, "An unhandled exception occurred.");
            // Set status code based on exception type
            var statusCode = exception switch
            {
                ArgumentNullException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Status = statusCode,
                Detail = exception.Message,
                Instance = httpContext.Request.Path
            };

            httpContext.Response.StatusCode = problemDetails.Status.Value;
            httpContext.Response.ContentType = "application/json";


            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
