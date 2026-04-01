import { NodeSDK } from '@opentelemetry/sdk-node';
import { getNodeAutoInstrumentations } from '@opentelemetry/auto-instrumentations-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-grpc';
import { OTLPMetricExporter } from '@opentelemetry/exporter-metrics-otlp-grpc';
import { OTLPLogExporter } from '@opentelemetry/exporter-logs-otlp-grpc';
import { PeriodicExportingMetricReader } from '@opentelemetry/sdk-metrics';
import { BatchLogRecordProcessor } from '@opentelemetry/sdk-logs';
import { resourceFromAttributes } from '@opentelemetry/resources';
import { ATTR_SERVICE_NAME } from '@opentelemetry/semantic-conventions';

const otlpEndpoint = process.env.OTEL_EXPORTER_OTLP_ENDPOINT || 'http://localhost:4317';

const resource = resourceFromAttributes({
    [ATTR_SERVICE_NAME]: 'interstyle-client',
});

const sdk = new NodeSDK({
    resource,
    traceExporter: new OTLPTraceExporter({
        url: otlpEndpoint,
    }),
    metricReader: new PeriodicExportingMetricReader({
        exporter: new OTLPMetricExporter({
            url: otlpEndpoint,
        }),
        exportIntervalMillis: 5000,
    }),
    logRecordProcessors: [
        new BatchLogRecordProcessor(
            new OTLPLogExporter({ url: otlpEndpoint })
        ),
    ],
    instrumentations: [getNodeAutoInstrumentations({
        '@opentelemetry/instrumentation-fs': {
            enabled: false,
        },
    })],
});

sdk.start();

console.log('OpenTelemetry configured for Aspire dashboard, endpoint:', otlpEndpoint);

process.on('SIGTERM', () => {
    sdk.shutdown()
        .then(() => console.log('Tracing terminated'))
        .catch((error: any) => console.log('Error terminating tracing', error))
        .finally(() => process.exit(0));
});