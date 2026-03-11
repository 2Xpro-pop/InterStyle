import { defaultLocale, isLocale } from '$lib/i18n/locale';
import type { Locale } from '$lib/i18n/locale';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({ url }) => {
	const raw = url.searchParams.get('lang') ?? '';
	const locale: Locale = isLocale(raw) ? raw : defaultLocale;
	return { locale };
};
