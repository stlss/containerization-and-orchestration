namespace LabDomain.Services;

public interface IMetricsService
{
    void RecordRequest(string method, string route, int statusCode, TimeSpan duration);
}
