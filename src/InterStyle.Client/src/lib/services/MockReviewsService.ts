import { demoReviews } from '$lib/data/demo-reviews';
import type { IReviewsService } from '$lib/services/IReviewsService';
import type { Review, ReviewPage, SubmitReviewRequest } from '$lib/types/review';

export class MockReviewsService implements IReviewsService {
	async getApprovedReviews(_: typeof fetch, limit: number): Promise<Review[]> {
		return demoReviews.slice(0, Math.max(0, limit));
	}

	async getApprovedReviewsPage(
		_: typeof fetch,
		page: number,
		pageSize: number
	): Promise<ReviewPage> {
		const safePage = Math.max(1, page);
		const safePageSize = Math.max(1, pageSize);
		const totalCount = demoReviews.length;
		const start = (safePage - 1) * safePageSize;
		const items = demoReviews.slice(start, start + safePageSize);
		const totalPages = Math.max(1, Math.ceil(totalCount / safePageSize));

		return {
			items,
			page: safePage,
			pageSize: safePageSize,
			totalCount,
			totalPages,
			hasNextPage: safePage < totalPages,
			hasPreviousPage: safePage > 1
		};
	}

	async submitReview(_: typeof fetch, _request: SubmitReviewRequest): Promise<{ id: string }> {
		return { id: crypto.randomUUID() };
	}
}
