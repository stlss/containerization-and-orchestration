using System.Globalization;
using LabDomain.Services;
using Prometheus;

namespace LabApplication.Services;

public sealed class MetricsService : IMetricsService
{
    private readonly Counter _requests = Metrics.CreateCounter(
        "lab_http_requests_total",
        "Total number of completed HTTP requests.",
        new CounterConfiguration { LabelNames = ["method", "route", "status_code"] });

    private readonly Counter _errors = Metrics.CreateCounter(
        "lab_http_errors_total",
        "Total number of HTTP requests resulting in a 5xx response or an unhandled exception.",
        new CounterConfiguration { LabelNames = ["method", "route"] });

    private readonly Histogram _duration = Metrics.CreateHistogram(
        "lab_http_request_duration_seconds",
        "HTTP request processing duration in seconds.",
        new HistogramConfiguration
        {
            LabelNames = ["method", "route"],
            Buckets = [0.005, 0.01, 0.025, 0.05, 0.1, 0.25, 0.5, 1, 2, 3, 4, 5, 10]
        });

    public void RecordRequest(string method, string route, int statusCode, TimeSpan duration)
    {
        _requests.WithLabels(method, route, statusCode.ToString(CultureInfo.InvariantCulture)).Inc();
        _errors.WithLabels(method, route).Inc(statusCode >= 500 ? 1 : 0);
        _duration.WithLabels(method, route).Observe(duration.TotalSeconds);
    }
}
