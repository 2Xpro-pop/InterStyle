/**
 * Lightweight structured logger for server-side code.
 *
 * Outputs one JSON object per line (ndjson) to stdout **and**
 * emits log records via the OpenTelemetry Logs API so they
 * appear in the Aspire dashboard. Trace context (traceId / spanId)
 * is attached automatically when a span is active.
 *
 * Usage:
 *   import { logger } from '$lib/logger';
 *   const log = logger.child({ component: 'CurtainsApi' });
 *   log.info({ url, itemCount: items.length }, 'Curtains fetched');
 */

import { trace } from '@opentelemetry/api';
import { logs, SeverityNumber } from '@opentelemetry/api-logs';
import type { LogAttributes } from '@opentelemetry/api-logs';

export type LogLevel = 'debug' | 'info' | 'warn' | 'error';

const LEVEL_SEVERITY: Record<LogLevel, number> = {
	debug: SeverityNumber.DEBUG,
	info: SeverityNumber.INFO,
	warn: SeverityNumber.WARN,
	error: SeverityNumber.ERROR,
};

const LEVEL_TEXT: Record<LogLevel, string> = {
	debug: 'DEBUG',
	info: 'INFO',
	warn: 'WARN',
	error: 'ERROR',
};

export interface StructuredLogger {
	debug(data: Record<string, unknown>, msg: string): void;
	info(data: Record<string, unknown>, msg: string): void;
	warn(data: Record<string, unknown>, msg: string): void;
	error(data: Record<string, unknown>, msg: string): void;
	child(bindings: Record<string, unknown>): StructuredLogger;
}

const otelLogger = logs.getLogger('interstyle-client');

class JsonLogger implements StructuredLogger {
	constructor(private readonly bindings: Record<string, unknown> = {}) {}

	child(extra: Record<string, unknown>): StructuredLogger {
		return new JsonLogger({ ...this.bindings, ...extra });
	}

	debug(data: Record<string, unknown>, msg: string) {
		this.write('debug', data, msg);
	}
	info(data: Record<string, unknown>, msg: string) {
		this.write('info', data, msg);
	}
	warn(data: Record<string, unknown>, msg: string) {
		this.write('warn', data, msg);
	}
	error(data: Record<string, unknown>, msg: string) {
		this.write('error', data, msg);
	}

	private write(level: LogLevel, data: Record<string, unknown>, msg: string) {
		const activeSpan = trace.getActiveSpan();
		const spanContext = activeSpan?.spanContext();

		const traceFields: Record<string, string> = {};
		if (spanContext?.traceId) {
			traceFields.traceId = spanContext.traceId;
			traceFields.spanId = spanContext.spanId;
		}

		// Emit via OpenTelemetry Logs API => OTLP exporter => Aspire
		otelLogger.emit({
			severityNumber: LEVEL_SEVERITY[level],
			severityText: LEVEL_TEXT[level],
			body: msg,
			attributes: { ...this.bindings, ...data, ...traceFields } as LogAttributes,
		});

		// Also write JSON to stdout for Docker / local dev
		const entry = {
			Timestamp: new Date().toISOString(),
			SeverityText: level,
			SeverityNumber: LEVEL_SEVERITY[level],
			Body: msg,
			...traceFields,
			...this.bindings,
			...data,
		};

		const fn = level === 'error' ? console.error : level === 'warn' ? console.warn : console.log;
		fn(JSON.stringify(entry));
	}
}

export const logger: StructuredLogger = new JsonLogger({
	'service.name': 'interstyle-client',
});
