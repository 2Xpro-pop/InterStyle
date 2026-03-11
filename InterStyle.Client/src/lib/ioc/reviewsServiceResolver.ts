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
			return reviews.length > 0 ? reviews : this.fallback.getApprovedReviews(fetchFn, limit);
		} catch {
			return this.fallback.getApprovedReviews(fetchFn, limit);
		}
	}

	async getApprovedReviewsPage(fetchFn: typeof fetch, page: number, pageSize: number) {
		try {
			const paged = await this.primary.getApprovedReviewsPage(fetchFn, page, pageSize);
			return paged.items.length > 0
				? paged
				: this.fallback.getApprovedReviewsPage(fetchFn, page, pageSize);
		} catch {
			return this.fallback.getApprovedReviewsPage(fetchFn, page, pageSize);
		}
	}

	async submitReview(fetchFn: typeof fetch, request: SubmitReviewRequest) {
		return this.primary.submitReview(fetchFn, request);
	}
}

export function resolveReviewsService(): IReviewsService {
	if (cachedService) {
		return cachedService;
	}

	const apiUrl = env.PUBLIC_REVIEWS_API_URL;
	const mockService = new MockReviewsService();
	cachedService = apiUrl
		? new FallbackReviewsService(new ApiReviewsService(apiUrl), mockService)
		: mockService;

	return cachedService;
}
