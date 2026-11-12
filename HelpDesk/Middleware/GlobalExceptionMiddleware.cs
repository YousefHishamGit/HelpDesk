using HelpDesk.Models;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware( RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
            _logger.LogError( ex, "Unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            KeyNotFoundException =>
                StatusCodes.Status404NotFound,

            UnauthorizedAccessException =>
                StatusCodes.Status403Forbidden,

            InvalidOperationException =>
                StatusCodes.Status400BadRequest,

            ArgumentException =>
                StatusCodes.Status400BadRequest,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        var response = new ApiErrorResponse
        {
            StatusCode = statusCode,
            Message = exception switch
            {
                KeyNotFoundException => exception.Message,
                UnauthorizedAccessException => exception.Message,
                InvalidOperationException => exception.Message,
                ArgumentException => exception.Message,
                _ => "An unexpected error occurred."
            }
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}