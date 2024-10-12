using System.Diagnostics.CodeAnalysis;

namespace FinancialManagementSystem.API.Middleware
{
    /// <summary>
    /// Middleware for handling global exceptions during request processing.
    /// Logs the exceptions and returns a 500 status code with an error message.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Constructor that initializes the middleware with the next delegate and logger.
        /// </summary>
        /// <param name="next">The next middleware in the request pipeline.</param>
        /// <param name="logger">Logger for logging exceptions and request information.</param>
        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Method invoked during the request processing to catch and log unhandled exceptions.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>A Task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation($"Processing request: {context.Request.Method} {context.Request.Path}");

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {ExceptionMessage}", ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError(ex.InnerException, "Inner exception: {InnerExceptionMessage}", ex.InnerException.Message);
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An error occurred processing your request.");
            }
        }
    }
}
