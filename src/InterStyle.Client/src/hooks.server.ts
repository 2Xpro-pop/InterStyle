import type { Handle } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';
import { isLocale, htmlLangCodes, defaultLocale } from '$lib/i18n/locale';
import { logger } from '$lib/logger';
import {
	tracer,
	httpRequestCounter,
	httpRequestDuration,
	httpErrorCounter,
} from '$lib/telemetry';
import { SpanStatusCode } from '@opentelemetry/api';

const log = logger.child({ component: 'Server' });

const requestLogger: Handle = async ({ event, resolve }) => {
	const start = Date.now();
	const { method } = event.request;
	const { pathname } = event.url;

	return tracer.startActiveSpan(`${method} ${pathname}`, {
		attributes: {
			'http.method': method,
			'http.route': pathname,
			'http.url': event.url.href,
		},
	}, async (span) => {
		httpRequestCounter.add(1, { method, route: pathname });
		log.info({ method, path: pathname }, 'Request started');

		let response: Response;
		try {
			response = await resolve(event);
		} catch (err) {
			const durationMs = Date.now() - start;
			httpRequestDuration.record(durationMs, { method, route: pathname, status: 500 });
			httpErrorCounter.add(1, { method, route: pathname });
			span.setStatus({ code: SpanStatusCode.ERROR, message: err instanceof Error ? err.message : String(err) });
			span.recordException(err instanceof Error ? err : new Error(String(err)));
			span.end();
			throw err;
		}

		const durationMs = Date.now() - start;
		const status = response.status;

		span.setAttribute('http.status_code', status);
		span.setAttribute('http.duration_ms', durationMs);
		httpRequestDuration.record(durationMs, { method, route: pathname, status });

		if (status >= 500) {
			httpErrorCounter.add(1, { method, route: pathname });
			span.setStatus({ code: SpanStatusCode.ERROR, message: `HTTP ${status}` });
			log.error({ method, path: pathname, status, durationMs }, 'Request completed with server error');
		} else if (status >= 400) {
			span.setStatus({ code: SpanStatusCode.OK });
			log.warn({ method, path: pathname, status, durationMs }, 'Request completed with client error');
		} else {
			span.setStatus({ code: SpanStatusCode.OK });
			log.info({ method, path: pathname, status, durationMs }, 'Request completed');
		}

		span.end();
		return response;
	});
};

const localeHandler: Handle = async ({ event, resolve }) => {
	const lang = event.url.searchParams.get('lang') ?? '';
	const locale = isLocale(lang) ? lang : defaultLocale;

	return resolve(event, {
		transformPageChunk: ({ html }) => html.replace('lang="ru"', `lang="${htmlLangCodes[locale]}"`)
	});
};

export const handle = sequence(requestLogger, localeHandler);
