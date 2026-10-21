using LabDomain.Services;

namespace LabApplication.Services;

public sealed class LabService2(HttpClient httpClient) : ILabService2
{
    private long _errorCount;

    public long RecordError() => Interlocked.Increment(ref _errorCount);

    public Task SlowAsync(CancellationToken cancellationToken) =>
        Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(1, 4)), cancellationToken);

    public async Task<int> LoadAsync(Uri uri, CancellationToken cancellationToken)
    {
        const int requestCount = 100;

        var requests = Enumerable.Range(0, requestCount).Select(async _ =>
        {
            using var response = await httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
        });

        await Task.WhenAll(requests);
        return requestCount;
    }
}
