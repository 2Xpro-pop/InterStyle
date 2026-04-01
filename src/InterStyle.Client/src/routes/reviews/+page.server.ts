import { resolveReviewsService } from '$lib/ioc/reviewsServiceResolver';
import { logger } from '$lib/logger';
import type { IReviewsService } from '$lib/services/IReviewsService';
import { env } from '$env/dynamic/public';
import { fail } from '@sveltejs/kit';
import { defaultLocale, isLocale } from '$lib/i18n/locale';
import { t } from '$lib/i18n/translations';

export const prerender = false;

const log = logger.child({ component: 'ReviewsPage' });

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
		reviewsPage,
		recaptchaSiteKey: env.PUBLIC_RECAPTCHA_SITE_KEY ?? ''
	};
}

export const actions = {
	default: async ({ request, fetch, url }) => {
		const raw = url.searchParams.get('lang') ?? '';
		const locale = isLocale(raw) ? raw : defaultLocale;

		const formData = await request.formData();
		const customerName = (formData.get('customerName') as string ?? '').trim();
		const ratingStr = formData.get('rating') as string ?? '';
		const comment = (formData.get('comment') as string ?? '').trim();
		const captchaToken = (formData.get('captchaToken') as string ?? '').trim();

		const errors: Record<string, string> = {};

		if (!customerName || customerName.length < 2) {
			errors.customerName = t(locale, 'validation.nameMin');
		} else if (customerName.length > 100) {
			errors.customerName = t(locale, 'validation.nameMax');
		}

		const rating = Number.parseInt(ratingStr, 10);
		if (!Number.isFinite(rating) || rating < 1 || rating > 5) {
			errors.rating = t(locale, 'validation.ratingRange');
		}

		if (!comment || comment.length < 5) {
			errors.comment = t(locale, 'validation.commentMin');
		} else if (comment.length > 2000) {
			errors.comment = t(locale, 'validation.commentMax');
		}

		if (!captchaToken) {
			errors.captcha = t(locale, 'validation.captcha');
		}

		if (Object.keys(errors).length > 0) {
			return fail(400, { errors, customerName, rating: ratingStr, comment });
		}

		try {
			const reviewsService = resolveReviewsService();
			await reviewsService.submitReview(fetch, { customerName, rating, comment, captchaToken });
			log.info({ customerName }, 'Review submitted successfully');
			return { success: true };
		} catch (err) {
			log.error({ error: err instanceof Error ? err.message : String(err), customerName }, 'Review submission failed');
			return fail(500, {
				errors: { form: t(locale, 'validation.submitError') } as Record<string, string>,
				customerName,
				rating: ratingStr,
				comment
			});
		}
	}
};
