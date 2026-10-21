using LabDomain.Services;

namespace LabApplication.Services;

public sealed class LabService2(HttpClient httpClient) : ILabService2
{
    public void Fail() => throw new InvalidOperationException("Intentional failure for Lab 2.");

    public Task SlowAsync(CancellationToken cancellationToken) =>
        Task.Delay(Random.Shared.Next(1000, 3001), cancellationToken);

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
