using System.Diagnostics;

namespace LabApi.Middlewares;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var startedAt = Stopwatch.GetTimestamp();

        logger.LogInformation(
            "HTTP {Method} {Path} started. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "HTTP {Method} {Path} failed after {ElapsedMs:F2} ms. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                context.TraceIdentifier);

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
            "HTTP {Method} {Path} returned {StatusCode} in {ElapsedMs:F2} ms. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
            context.TraceIdentifier);
    }
}
