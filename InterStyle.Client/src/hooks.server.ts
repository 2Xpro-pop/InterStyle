import type { Handle } from '@sveltejs/kit';
import { isLocale, htmlLangCodes, defaultLocale } from '$lib/i18n/locale';

export const handle: Handle = async ({ event, resolve }) => {
	const lang = event.url.searchParams.get('lang') ?? '';
	const locale = isLocale(lang) ? lang : defaultLocale;

	return resolve(event, {
		transformPageChunk: ({ html }) => html.replace('lang="ru"', `lang="${htmlLangCodes[locale]}"`)
	});
};
