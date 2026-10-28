using System.Diagnostics;

namespace LabApplication.Diagnostics;

public static class LabTelemetry
{
    public const string SourceName = "LabApplication";

    public static readonly ActivitySource ActivitySource = new(SourceName);
}
