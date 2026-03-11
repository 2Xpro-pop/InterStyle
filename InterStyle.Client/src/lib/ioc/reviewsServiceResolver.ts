import { env } from '$env/dynamic/public';
import { ApiReviewsService } from '$lib/services/ApiReviewsService';
import type { IReviewsService } from '$lib/services/IReviewsService';
import { MockReviewsService } from '$lib/services/MockReviewsService';
import type { SubmitReviewRequest } from '$lib/types/review';

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
				console.warn('[ReviewsService] API returned empty reviews, falling back to mock');
			}
			return reviews.length > 0 ? reviews : this.fallback.getApprovedReviews(fetchFn, limit);
		} catch (err) {
			console.error('[ReviewsService] getApprovedReviews failed, using mock:', err instanceof Error ? err.message : err);
			return this.fallback.getApprovedReviews(fetchFn, limit);
		}
	}

	async getApprovedReviewsPage(fetchFn: typeof fetch, page: number, pageSize: number) {
		try {
			const paged = await this.primary.getApprovedReviewsPage(fetchFn, page, pageSize);
			if (paged.items.length === 0) {
				console.warn(`[ReviewsService] API returned empty page ${page}, falling back to mock`);
			}
			return paged.items.length > 0
				? paged
				: this.fallback.getApprovedReviewsPage(fetchFn, page, pageSize);
		} catch (err) {
			console.error('[ReviewsService] getApprovedReviewsPage failed, using mock:', err instanceof Error ? err.message : err);
			return this.fallback.getApprovedReviewsPage(fetchFn, page, pageSize);
		}
	}

	async submitReview(fetchFn: typeof fetch, request: SubmitReviewRequest) {
		console.log(`[ReviewsService] Submitting review from "${request.customerName}"`);
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
		console.log(`[ReviewsService] Using API gateway: ${apiUrl}`);
		cachedService = new FallbackReviewsService(new ApiReviewsService(apiUrl), mockService);
	} else {
		console.warn('[ReviewsService] PUBLIC_API_GATEWAY_URL not set, using mock');
		cachedService = mockService;
	}

	return cachedService;
}
