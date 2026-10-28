using System.Diagnostics;
using LabApplication.Diagnostics;
using LabDomain.Services;

namespace LabApplication.Services;

public sealed class LabService2(HttpClient httpClient) : ILabService2
{
    public void Fail()
    {
        const string message = "Intentional failure for Lab 2.";
        Activity.Current?.SetStatus(ActivityStatusCode.Error, message);
        throw new InvalidOperationException(message);
    }

    public async Task SlowAsync(CancellationToken cancellationToken)
    {
        using var activity = LabTelemetry.ActivitySource.StartActivity("slow-op");
        await Task.Delay(Random.Shared.Next(1000, 3001), cancellationToken);
    }

    public async Task<int> LoadAsync(Uri uri, int requestCount, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(requestCount);

        var requests = Enumerable.Range(0, requestCount).Select(async _ =>
        {
            using var response = await httpClient.GetAsync(uri, cancellationToken);
        });

        await Task.WhenAll(requests);
        return requestCount;
    }
}
