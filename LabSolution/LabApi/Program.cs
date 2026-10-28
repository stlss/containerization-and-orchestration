using LabApi.Middlewares;
using LabApplication.Diagnostics;
using LabApplication.Services;
using LabDomain.Services;
using Prometheus;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace LabApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddJsonConsole(options =>
        {
            options.IncludeScopes = true;
            options.UseUtcTimestamp = true;
            options.TimestampFormat = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";
        });

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                builder.Configuration["OTEL_SERVICE_NAME"] ?? "lab-api"))
            .WithTracing(tracing => tracing
                .AddSource(LabTelemetry.SourceName)
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.RecordException = true;
                    options.Filter = context => context.Request.Path != "/metrics";
                })
                .AddHttpClientInstrumentation()
                .AddOtlpExporter());

        builder.Services.AddSingleton<ILabService1, LabService1>();
        builder.Services.AddSingleton<ILabService2, LabService2>();
        builder.Services.AddSingleton<IMetricsService, MetricsService>();

        builder.Services.AddSingleton(_ => new HttpClient());

        var app = builder.Build();

        app.UseRouting();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<RequestMetricsMiddleware>();

        app.MapControllers();
        app.MapMetrics();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Logger.LogInformation("{Application} starting. PID: {ProcessId}.", nameof(LabApi), Environment.ProcessId);

        app.Run();
    }
}
