using LabDomain.Services;

namespace LabApplication.Services;

public sealed class LabService1 : ILabService1
{
    private readonly List<byte[]> _byteArrays = [];

    public string GetHealthStatus() => "ok";

    public void EatMemory(int mb)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(mb);

        for (var i = 0; i < mb; i++)
        {
            var block = new byte[1024 * 1024];
            Array.Fill(block, (byte)1);
            _byteArrays.Add(block);
        }
    }

    public void BurnCpu()
    {
        var thread = new Thread(OnInfinityCycle)  { IsBackground = true };
        thread.Start();

        return;

        void OnInfinityCycle()
        {
            while (true)
            {
                Thread.SpinWait(1024);
            }
        }
    }
}
