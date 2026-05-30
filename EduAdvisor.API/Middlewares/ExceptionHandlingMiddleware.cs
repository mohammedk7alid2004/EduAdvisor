using EduAdvisor.Application.Common.Abstractions;
using EduAdvisor.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        object result;

        switch (exception)
        {
            case ValidationException ve:
                status = HttpStatusCode.BadRequest;
                var errors = ve.Errors.Select(e => e.ErrorMessage).ToList();
                result = Result<object>.Failure(errors.FirstOrDefault() ?? "Validation failed", 400);
                break;

            case NotFoundException ne:
                status = HttpStatusCode.NotFound;
                result = Result<object>.Failure(ne.Message, 404);
                break;

            default:
                status = HttpStatusCode.InternalServerError;
                result = Result<object>.Error("An unexpected error occurred.", 500);
                break;
        }

        var logLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {exception.GetType().Name} | {exception.Message} | {exception.StackTrace}";
        var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "errors.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
        await File.AppendAllTextAsync(logPath, logLine + Environment.NewLine);

        logger.LogError(exception, exception.Message);

        context.Response.ContentType = "EduAdvisor.Application/json";
        context.Response.StatusCode = (int)status;

        await context.Response.WriteAsync(JsonSerializer.Serialize(result, JsonOptions));
    }
}
