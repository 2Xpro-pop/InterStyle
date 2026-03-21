import { env } from '$env/dynamic/public';
import { logger } from '$lib/logger';
import { ApiReviewsService } from '$lib/services/ApiReviewsService';
import type { IReviewsService } from '$lib/services/IReviewsService';
import { MockReviewsService } from '$lib/services/MockReviewsService';
import type { SubmitReviewRequest } from '$lib/types/review';

const log = logger.child({ component: 'ReviewsService' });

let cachedService: IReviewsService | null = null;

class FallbackReviewsService implements IReviewsService {
	constructor(
		private readonly primary: IReviewsService,
		private readonly fallback: IReviewsService
	) {}

	async getApprovedReviews(fetchFn: typeof fetch, limit: number) {
		try {
			const reviews = await this.primary.getApprovedReviews(fetchFn, limit);
			if (reviews.length === 0) {
				log.warn({ limit }, 'API returned empty reviews, falling back to mock');
			}
			return reviews.length > 0 ? reviews : this.fallback.getApprovedReviews(fetchFn, limit);
		} catch (err) {
			log.error({ error: err instanceof Error ? err.message : String(err), limit }, 'getApprovedReviews failed, using mock fallback');
			return this.fallback.getApprovedReviews(fetchFn, limit);
		}
	}

	async getApprovedReviewsPage(fetchFn: typeof fetch, page: number, pageSize: number) {
		try {
			const paged = await this.primary.getApprovedReviewsPage(fetchFn, page, pageSize);
			if (paged.items.length === 0) {
				log.warn({ page, pageSize }, 'API returned empty page, falling back to mock');
			}
			return paged.items.length > 0
				? paged
				: this.fallback.getApprovedReviewsPage(fetchFn, page, pageSize);
		} catch (err) {
			log.error({ error: err instanceof Error ? err.message : String(err), page, pageSize }, 'getApprovedReviewsPage failed, using mock fallback');
			return this.fallback.getApprovedReviewsPage(fetchFn, page, pageSize);
		}
	}

	async submitReview(fetchFn: typeof fetch, request: SubmitReviewRequest) {
		log.info({ customerName: request.customerName }, 'Submitting review');
		return this.primary.submitReview(fetchFn, request);
	}
}

export function resolveReviewsService(): IReviewsService {
	if (cachedService) {
		return cachedService;
	}

	const apiUrl = env.PUBLIC_API_GATEWAY_URL;
	const mockService = new MockReviewsService();

	if (apiUrl) {
		log.info({ apiUrl }, 'Resolved with API gateway');
		cachedService = new FallbackReviewsService(new ApiReviewsService(apiUrl), mockService);
	} else {
		log.warn({}, 'PUBLIC_API_GATEWAY_URL not set, using mock service');
		cachedService = mockService;
	}

	return cachedService;
}
