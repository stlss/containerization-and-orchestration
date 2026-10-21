namespace LabDomain.Services;

public interface ILabService2
{
    void Fail();

    Task SlowAsync(CancellationToken cancellationToken);

    Task<int> LoadAsync(Uri uri, int requestCount, CancellationToken cancellationToken);
}
