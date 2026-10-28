using System.Diagnostics;

namespace LabApi.Middlewares;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();

        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["trace_id"] = Activity.Current?.TraceId.ToHexString(),
            ["request_id"] = context.TraceIdentifier
        });

        logger.LogInformation(
            "HTTP {Method} {Path} started.",
            context.Request.Method,
            context.Request.Path);

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "HTTP {Method} {Path} failed with {StatusCode} after {ElapsedMs:F2} ms.",
                context.Request.Method,
                context.Request.Path,
                StatusCodes.Status500InternalServerError,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);

            throw;
        }

        var statusCode = context.Response.StatusCode;

        var logLevel = statusCode switch
        {
            >= 500 => LogLevel.Error,
            >= 400 => LogLevel.Warning,
            _ => LogLevel.Information
        };

        logger.Log(
            logLevel,
            "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMs:F2} ms.",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds);
    }
}
