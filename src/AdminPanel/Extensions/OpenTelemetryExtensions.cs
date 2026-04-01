using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AdminPanel.Extensions;

public static class OpenTelemetryExtensions
{
    public static WebAssemblyHostBuilder ConfigureOpenTelemetry(
        this WebAssemblyHostBuilder builder,
        params string[] activitySourceNames)
    {

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("AdminPanel"))
            .WithTracing(tracing =>
            {
                tracing.SetSampler(new AlwaysOnSampler());
                tracing.AddHttpClientInstrumentation();

                foreach (var source in activitySourceNames)
                {
                    tracing.AddSource(source);
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddHttpClientInstrumentation();
            });

        return builder;
    }
}
