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
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var hasOtlp = !string.IsNullOrWhiteSpace(otlpEndpoint);

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            if (hasOtlp)
            {
                logging.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint!);
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                });
            }
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

                if (hasOtlp)
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint!);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddHttpClientInstrumentation();

                if (hasOtlp)
                {
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint!);
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    });
                }
            });

        return builder;
    }
}
