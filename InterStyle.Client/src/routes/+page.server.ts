import { resolveCurtainsService } from '$lib/ioc/curtainsServiceResolver';
import { resolveReviewsService } from '$lib/ioc/reviewsServiceResolver';
import type { ICurtainsService } from '$lib/services/ICurtainsService';
import type { IReviewsService } from '$lib/services/IReviewsService';

export const prerender = true;

async function getHomeCurtains(service: ICurtainsService, fetchFn: typeof fetch) {
	const curtains = await service.getAllCurtains(fetchFn);
	return curtains.slice(0, 3);
}

async function getHomeReviews(service: IReviewsService, fetchFn: typeof fetch) {
	return service.getApprovedReviews(fetchFn, 3);
}

export async function load({ fetch }) {
	const curtainsService = resolveCurtainsService();
	const reviewsService = resolveReviewsService();

	const [curtains, reviews] = await Promise.all([
		getHomeCurtains(curtainsService, fetch),
		getHomeReviews(reviewsService, fetch)
	]);

	return {
		curtains,
		reviews
	};
}
