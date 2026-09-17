namespace HMS.API.MiddleWare
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new { success = false, message = "", errors = new List<string>() };

            switch (exception)
            {
                case ArgumentNullException:
                case ArgumentException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    response = new { success = false, message = "Invalid input provided", errors = new List<string> { exception.Message } };
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    response = new { success = false, message = "Resource not found", errors = new List<string> { exception.Message } };
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response = new { success = false, message = "Operation conflict", errors = new List<string> { exception.Message } };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    response = new { success = false, message = "Unauthorized access", errors = new List<string> { exception.Message } };
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    response = new { success = false, message = "An unexpected error occurred", errors = new List<string>() };
                    break;
            }

            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
