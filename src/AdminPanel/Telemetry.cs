using System.Diagnostics;

namespace AdminPanel;

public static class Telemetry
{
    public static readonly ActivitySource ActivitySource = new("AdminPanel");
}
