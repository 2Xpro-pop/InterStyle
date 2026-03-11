import type { IReviewsService } from '$lib/services/IReviewsService';
import type { Review, ReviewPage, SubmitReviewRequest } from '$lib/types/review';

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
		const response = await fetchFn(
			`${this.baseUrl}/api/reviews?api-version=1.0&page=${safePage}&pageSize=${safePageSize}`
		);

		if (!response.ok) {
			throw new Error('Reviews API is unavailable');
		}

		const payload = (await response.json()) as PagedResultDto<ReviewApiDto>;
		const items = Array.isArray(payload.items) ? payload.items : [];
		const totalCount = typeof payload.totalCount === 'number' ? payload.totalCount : items.length;
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
	}

	async submitReview(fetchFn: typeof fetch, request: SubmitReviewRequest): Promise<{ id: string }> {
		const response = await fetchFn(`${this.baseUrl}/api/reviews?api-version=1.0`, {
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(request)
		});

		if (!response.ok) {
			throw new Error('Не удалось отправить отзыв');
		}

		const data = await response.json();
		return { id: data.id ?? '' };
	}
}
