/**
 * Lightweight structured logger for server-side code.
 *
 * Outputs one JSON object per line (ndjson), which Aspire /
 * Docker log drivers can parse automatically.
 *
 * Usage:
 *   import { logger } from '$lib/logger';
 *   const log = logger.child({ component: 'CurtainsApi' });
 *   log.info({ url, itemCount: items.length }, 'Curtains fetched');
 */

export type LogLevel = 'debug' | 'info' | 'warn' | 'error';

const LEVEL_SEVERITY: Record<LogLevel, number> = {
	debug: 5,
	info: 9,
	warn: 13,
	error: 17
};

export interface StructuredLogger {
	debug(data: Record<string, unknown>, msg: string): void;
	info(data: Record<string, unknown>, msg: string): void;
	warn(data: Record<string, unknown>, msg: string): void;
	error(data: Record<string, unknown>, msg: string): void;
	child(bindings: Record<string, unknown>): StructuredLogger;
}

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
		const entry = {
			Timestamp: new Date().toISOString(),
			SeverityText: level,
			SeverityNumber: LEVEL_SEVERITY[level],
			Body: msg,
			...this.bindings,
			...data
		};

		const fn = level === 'error' ? console.error : level === 'warn' ? console.warn : console.log;
		fn(JSON.stringify(entry));
	}
}

export const logger: StructuredLogger = new JsonLogger({
	'service.name': 'interstyle-client'
});
