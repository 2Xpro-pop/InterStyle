import { trace, metrics, SpanStatusCode, type Span } from '@opentelemetry/api';

const SERVICE_NAME = 'interstyle-client';

export const tracer = trace.getTracer(SERVICE_NAME);
export const meter = metrics.getMeter(SERVICE_NAME);

// ── Metrics ──────────────────────────────────────────────────
export const httpRequestCounter = meter.createCounter('http.server.request.count', {
	description: 'Total incoming HTTP requests',
});

export const httpRequestDuration = meter.createHistogram('http.server.request.duration', {
	description: 'Duration of incoming HTTP requests in ms',
	unit: 'ms',
});

export const httpErrorCounter = meter.createCounter('http.server.error.count', {
	description: 'Total HTTP server errors (5xx)',
});

export const apiCallCounter = meter.createCounter('http.client.api.call.count', {
	description: 'Total outgoing API calls',
});

export const apiCallDuration = meter.createHistogram('http.client.api.call.duration', {
	description: 'Duration of outgoing API calls in ms',
	unit: 'ms',
});

export const apiCallErrorCounter = meter.createCounter('http.client.api.error.count', {
	description: 'Total failed outgoing API calls',
});

// ── Helpers ──────────────────────────────────────────────────

/**
 * Run an async function inside a new span, automatically recording exceptions
 * and setting status on failure.
 */
export async function withSpan<T>(
	name: string,
	attributes: Record<string, string | number | boolean>,
	fn: (span: Span) => Promise<T>,
): Promise<T> {
	return tracer.startActiveSpan(name, { attributes }, async (span) => {
		try {
			const result = await fn(span);
			span.setStatus({ code: SpanStatusCode.OK });
			return result;
		} catch (err) {
			span.setStatus({ code: SpanStatusCode.ERROR, message: err instanceof Error ? err.message : String(err) });
			span.recordException(err instanceof Error ? err : new Error(String(err)));
			throw err;
		} finally {
			span.end();
		}
	});
}
