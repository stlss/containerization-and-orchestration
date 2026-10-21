namespace LabDomain.Services;

public interface ILabService2
{
    long RecordError();

    Task SlowAsync(CancellationToken cancellationToken);

    Task<int> LoadAsync(Uri uri, CancellationToken cancellationToken);
}
