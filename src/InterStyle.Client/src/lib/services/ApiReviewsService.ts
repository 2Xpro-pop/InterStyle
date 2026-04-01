import type { IReviewsService } from '$lib/services/IReviewsService';
import { logger } from '$lib/logger';
import { withSpan, apiCallCounter, apiCallDuration, apiCallErrorCounter } from '$lib/telemetry';
import type { Review, ReviewPage, SubmitReviewRequest } from '$lib/types/review';

const log = logger.child({ component: 'ReviewsApi' });

interface ReviewApiDto {
	id?: string;
	customerName?: string;
	rating?: number;
	comment?: string;
	isApproved?: boolean;
	createdAtUtc?: string;
	approvedAtUtc?: string | null;
}

interface PagedResultDto<T> {
	items?: T[];
	page?: number;
	pageSize?: number;
	totalCount?: number;
}

function mapApiReview(item: ReviewApiDto, index: number): Review {
	return {
		id: item.id ?? `api-review-${index + 1}`,
		customerName: item.customerName ?? 'Клиент InterStyle',
		rating: typeof item.rating === 'number' ? item.rating : 5,
		comment: item.comment ?? 'Отзыв без текста',
		isApproved: item.isApproved ?? true,
		createdAtUtc: item.createdAtUtc ?? new Date().toISOString(),
		approvedAtUtc: item.approvedAtUtc ?? null
	};
}

export class ApiReviewsService implements IReviewsService {
	constructor(private readonly baseUrl: string) {}

	async getApprovedReviews(fetchFn: typeof fetch, limit: number): Promise<Review[]> {
		const paged = await this.getApprovedReviewsPage(fetchFn, 1, limit);
		return paged.items;
	}

	async getApprovedReviewsPage(
		fetchFn: typeof fetch,
		page: number,
		pageSize: number
	): Promise<ReviewPage> {
		const safePage = Math.max(1, page);
		const safePageSize = Math.max(1, pageSize);
		const reqUrl = `${this.baseUrl}/api/reviews?api-version=1.0&page=${safePage}&pageSize=${safePageSize}`;

		return withSpan('ReviewsApi.getApprovedReviewsPage', { 'api.url': reqUrl, 'api.page': safePage, 'api.pageSize': safePageSize }, async (span) => {
			const start = Date.now();
			apiCallCounter.add(1, { api: 'reviews', operation: 'getReviewsPage' });
			log.info({ url: reqUrl, page: safePage, pageSize: safePageSize }, 'Fetching reviews page');

			let response: Response;
			try {
				response = await fetchFn(reqUrl);
			} catch (err) {
				apiCallErrorCounter.add(1, { api: 'reviews', operation: 'getReviewsPage' });
				apiCallDuration.record(Date.now() - start, { api: 'reviews', operation: 'getReviewsPage', status: 0 });
				throw err;
			}

			span.setAttribute('http.status_code', response.status);
			apiCallDuration.record(Date.now() - start, { api: 'reviews', operation: 'getReviewsPage', status: response.status });

			if (!response.ok) {
				apiCallErrorCounter.add(1, { api: 'reviews', operation: 'getReviewsPage' });
				log.error({ url: reqUrl, status: response.status, statusText: response.statusText }, 'Reviews API request failed');
				throw new Error('Reviews API is unavailable');
			}

			const payload = (await response.json()) as PagedResultDto<ReviewApiDto>;
			const items = Array.isArray(payload.items) ? payload.items : [];
			const totalCount = typeof payload.totalCount === 'number' ? payload.totalCount : items.length;
			span.setAttribute('api.item_count', items.length);
			span.setAttribute('api.total_count', totalCount);
			log.info({ itemCount: items.length, totalCount }, 'Reviews page fetched successfully');

			const currentPage = typeof payload.page === 'number' ? payload.page : safePage;
			const currentPageSize = typeof payload.pageSize === 'number' ? payload.pageSize : safePageSize;
			const totalPages = Math.max(1, Math.ceil(totalCount / currentPageSize));

			return {
				items: items.map(mapApiReview),
				page: currentPage,
				pageSize: currentPageSize,
				totalCount,
				totalPages,
				hasNextPage: currentPage < totalPages,
				hasPreviousPage: currentPage > 1
			};
		});
	}

	async submitReview(fetchFn: typeof fetch, request: SubmitReviewRequest): Promise<{ id: string }> {
		const reqUrl = `${this.baseUrl}/api/reviews?api-version=1.0`;

		return withSpan('ReviewsApi.submitReview', { 'api.url': reqUrl, 'api.method': 'POST' }, async (span) => {
			const start = Date.now();
			apiCallCounter.add(1, { api: 'reviews', operation: 'submitReview' });
			log.info({ url: reqUrl, customerName: request.customerName }, 'Submitting review');

			let response: Response;
			try {
				response = await fetchFn(reqUrl, {
					method: 'POST',
					headers: { 'Content-Type': 'application/json' },
					body: JSON.stringify(request)
				});
			} catch (err) {
				apiCallErrorCounter.add(1, { api: 'reviews', operation: 'submitReview' });
				apiCallDuration.record(Date.now() - start, { api: 'reviews', operation: 'submitReview', status: 0 });
				throw err;
			}

			span.setAttribute('http.status_code', response.status);
			apiCallDuration.record(Date.now() - start, { api: 'reviews', operation: 'submitReview', status: response.status });

			if (!response.ok) {
				apiCallErrorCounter.add(1, { api: 'reviews', operation: 'submitReview' });
				log.error({ url: reqUrl, status: response.status, statusText: response.statusText }, 'Review submission failed');
				throw new Error('Не удалось отправить отзыв');
			}

			const data = await response.json();
			log.info({ reviewId: data.id ?? '' }, 'Review submitted successfully');
			return { id: data.id ?? '' };
		});
	}
}
