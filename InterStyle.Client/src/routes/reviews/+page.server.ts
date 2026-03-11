import { resolveReviewsService } from '$lib/ioc/reviewsServiceResolver';
import type { IReviewsService } from '$lib/services/IReviewsService';
import { fail } from '@sveltejs/kit';

export const prerender = false;

async function getReviewsPageData(
	service: IReviewsService,
	fetchFn: typeof fetch,
	page: number,
	pageSize: number
) {
	return service.getApprovedReviewsPage(fetchFn, page, pageSize);
}

function toPositiveInt(value: string | null, fallback: number): number {
	if (!value) {
		return fallback;
	}

	const parsed = Number.parseInt(value, 10);
	return Number.isFinite(parsed) && parsed > 0 ? parsed : fallback;
}

export async function load({ fetch, url }) {
	const reviewsService = resolveReviewsService();
	const page = toPositiveInt(url.searchParams.get('page'), 1);
	const pageSize = Math.min(24, toPositiveInt(url.searchParams.get('pageSize'), 9));
	const reviewsPage = await getReviewsPageData(reviewsService, fetch, page, pageSize);

	return {
		reviewsPage
	};
}

export const actions = {
	default: async ({ request, fetch }) => {
		const formData = await request.formData();
		const customerName = (formData.get('customerName') as string ?? '').trim();
		const ratingStr = formData.get('rating') as string ?? '';
		const comment = (formData.get('comment') as string ?? '').trim();

		const errors: Record<string, string> = {};

		if (!customerName || customerName.length < 2) {
			errors.customerName = 'Имя должно содержать минимум 2 символа.';
		} else if (customerName.length > 100) {
			errors.customerName = 'Имя не должно превышать 100 символов.';
		}

		const rating = Number.parseInt(ratingStr, 10);
		if (!Number.isFinite(rating) || rating < 1 || rating > 5) {
			errors.rating = 'Оценка должна быть от 1 до 5.';
		}

		if (!comment || comment.length < 5) {
			errors.comment = 'Комментарий должен содержать минимум 5 символов.';
		} else if (comment.length > 2000) {
			errors.comment = 'Комментарий не должен превышать 2000 символов.';
		}

		if (Object.keys(errors).length > 0) {
			return fail(400, { errors, customerName, rating: ratingStr, comment });
		}

		try {
			const reviewsService = resolveReviewsService();
			await reviewsService.submitReview(fetch, { customerName, rating, comment });
			return { success: true };
		} catch {
			return fail(500, {
				errors: { form: 'Не удалось отправить отзыв. Попробуйте позже.' } as Record<string, string>,
				customerName,
				rating: ratingStr,
				comment
			});
		}
	}
};
