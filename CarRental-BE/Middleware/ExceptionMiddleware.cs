using CarRental_BE.Exceptions;

namespace CarRental_BE.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogWarning(ex, "Handled AppException");
                await WriteResponse(context, ex.StatusCode, ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled Exception");
                await WriteResponse(context, 500, "An unexpected error occurred.", 9999);
            }
        }

        private async Task WriteResponse(HttpContext context, int statusCode, string message, int errorCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                status = statusCode,
                message,
                code = errorCode
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
