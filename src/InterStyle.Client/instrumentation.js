import { NodeSDK } from '@opentelemetry/sdk-node';
import { OTLPTraceExporter } from '@opentelemetry/exporter-trace-otlp-http';
import { HttpInstrumentation } from '@opentelemetry/instrumentation-http';
import { Resource } from '@opentelemetry/resources';
import { ATTR_SERVICE_NAME } from '@opentelemetry/semantic-conventions';

const otlpEndpoint = process.env.OTEL_EXPORTER_OTLP_ENDPOINT;

const resource = new Resource({
	[ATTR_SERVICE_NAME]: 'interstyle-client'
});

const sdk = new NodeSDK({
	resource,
	traceExporter: otlpEndpoint
		? new OTLPTraceExporter({ url: `${otlpEndpoint}/v1/traces` })
		: undefined,
	instrumentations: [new HttpInstrumentation()]
});

sdk.start();

process.on('SIGTERM', () => sdk.shutdown());
