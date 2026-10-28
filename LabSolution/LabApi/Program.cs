using LabApi.Middlewares;
using LabApplication.Services;
using LabDomain.Services;
using Prometheus;

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
