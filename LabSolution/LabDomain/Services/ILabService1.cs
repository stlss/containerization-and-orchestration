namespace LabDomain.Services;

public interface ILabService1
{
    string GetHealthStatus();

    void EatMemory(int mb);

    void BurnCpu();
}
