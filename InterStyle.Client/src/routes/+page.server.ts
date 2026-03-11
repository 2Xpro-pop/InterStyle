import { resolveCurtainsService } from '$lib/ioc/curtainsServiceResolver';
import { resolveReviewsService } from '$lib/ioc/reviewsServiceResolver';
import { defaultLocale, isLocale, cultureCodes } from '$lib/i18n/locale';
import type { ICurtainsService } from '$lib/services/ICurtainsService';
import type { IReviewsService } from '$lib/services/IReviewsService';

export const prerender = false;

async function getHomeCurtains(service: ICurtainsService, fetchFn: typeof fetch, culture: string) {
	const curtains = await service.getAllCurtains(fetchFn, culture);
	return curtains.slice(0, 3);
}

async function getHomeReviews(service: IReviewsService, fetchFn: typeof fetch) {
	return service.getApprovedReviews(fetchFn, 3);
}

export async function load({ fetch, url }) {
	const raw = url.searchParams.get('lang') ?? '';
	const locale = isLocale(raw) ? raw : defaultLocale;
	const culture = cultureCodes[locale];

	const curtainsService = resolveCurtainsService();
	const reviewsService = resolveReviewsService();

	const [curtains, reviews] = await Promise.all([
		getHomeCurtains(curtainsService, fetch, culture),
		getHomeReviews(reviewsService, fetch)
	]);

	return {
		curtains,
		reviews
	};
}
