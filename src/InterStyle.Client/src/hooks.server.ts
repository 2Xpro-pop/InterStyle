import type { Handle } from '@sveltejs/kit';
import { sequence } from '@sveltejs/kit/hooks';
import { isLocale, htmlLangCodes, defaultLocale } from '$lib/i18n/locale';
import { logger } from '$lib/logger';

const log = logger.child({ component: 'Server' });

const requestLogger: Handle = async ({ event, resolve }) => {
	const start = Date.now();
	const { method } = event.request;
	const { pathname } = event.url;

	log.info({ method, path: pathname }, 'Request started');

	const response = await resolve(event);

	const durationMs = Date.now() - start;
	const status = response.status;

	if (status >= 500) {
		log.error({ method, path: pathname, status, durationMs }, 'Request completed with server error');
	} else if (status >= 400) {
		log.warn({ method, path: pathname, status, durationMs }, 'Request completed with client error');
	} else {
		log.info({ method, path: pathname, status, durationMs }, 'Request completed');
	}

	return response;
};

const localeHandler: Handle = async ({ event, resolve }) => {
	const lang = event.url.searchParams.get('lang') ?? '';
	const locale = isLocale(lang) ? lang : defaultLocale;

	return resolve(event, {
		transformPageChunk: ({ html }) => html.replace('lang="ru"', `lang="${htmlLangCodes[locale]}"`)
	});
};

export const handle = sequence(requestLogger, localeHandler);
