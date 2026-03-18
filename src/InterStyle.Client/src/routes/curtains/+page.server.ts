import { resolveCurtainsService } from '$lib/ioc/curtainsServiceResolver';
import { defaultLocale, isLocale, cultureCodes } from '$lib/i18n/locale';
import type { ICurtainsService } from '$lib/services/ICurtainsService';

export const prerender = false;

async function getCatalogCurtains(service: ICurtainsService, fetchFn: typeof fetch, culture: string) {
	return service.getAllCurtains(fetchFn, culture);
}

export async function load({ fetch, url }) {
	const raw = url.searchParams.get('lang') ?? '';
	const locale = isLocale(raw) ? raw : defaultLocale;
	const culture = cultureCodes[locale];

	const curtainsService = resolveCurtainsService();
	const curtains = await getCatalogCurtains(curtainsService, fetch, culture);

	return {
		curtains
	};
}
