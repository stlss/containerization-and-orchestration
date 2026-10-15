using LabApi.Middlewares;
using LabApplication.Services;
using LabDomain.Services;

namespace LabApi;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddSimpleConsole(options =>
        {
            options.SingleLine = true;
            options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss zzz] ";
        });

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<ILabService1, LabService1>();

        var app = builder.Build();

        app.UseMiddleware<RequestLoggingMiddleware>();
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Logger.LogInformation("{Application} starting. PID: {ProcessId}.", nameof(LabApi), Environment.ProcessId);

        app.Run();
    }
}
