using System.Diagnostics;
using LabDomain.Services;

namespace LabApi.Middlewares;

public sealed class RequestMetricsMiddleware(RequestDelegate next, IMetricsService metricsService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == "/metrics")
        {
            await next(context);
            return;
        }

        var startedAt = Stopwatch.GetTimestamp();
        var failed = false;

        try
        {
            await next(context);
        }
        catch
        {
            failed = true;
            throw;
        }
        finally
        {
            var routePattern = (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText;
            var route = routePattern is null ? "unmatched" : $"/{routePattern.TrimStart('/')}";
            var statusCode = failed ? StatusCodes.Status500InternalServerError : context.Response.StatusCode;

            metricsService.RecordRequest(
                context.Request.Method,
                route,
                statusCode,
                Stopwatch.GetElapsedTime(startedAt));
        }
    }
}
