import type { Review, ReviewPage, SubmitReviewRequest } from '$lib/types/review';

export interface IReviewsService {
	getApprovedReviews(fetchFn: typeof fetch, limit: number): Promise<Review[]>;
	getApprovedReviewsPage(fetchFn: typeof fetch, page: number, pageSize: number): Promise<ReviewPage>;
	submitReview(fetchFn: typeof fetch, request: SubmitReviewRequest): Promise<{ id: string }>;
}
