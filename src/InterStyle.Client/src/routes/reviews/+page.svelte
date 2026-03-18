<script lang="ts">
	import type { PageData, ActionData } from './$types';
	import { enhance } from '$app/forms';
	import { page } from '$app/state';
	import ReviewCard from '$lib/components/ReviewCard.svelte';
	import { t } from '$lib/i18n/translations';
	import type { Locale } from '$lib/i18n/locale';

	let { data, form }: { data: PageData; form: ActionData } = $props();

	const locale = () => (page.data as { locale: Locale }).locale;

	const langParam = () => {
		const lang = (page.data as { locale: Locale }).locale;
		return lang === 'ru' ? '' : `&lang=${lang}`;
	};

	const prevHref = () =>
		`/reviews?page=${data.reviewsPage.page - 1}&pageSize=${data.reviewsPage.pageSize}${langParam()}`;
	const nextHref = () =>
		`/reviews?page=${data.reviewsPage.page + 1}&pageSize=${data.reviewsPage.pageSize}${langParam()}`;

	let selectedRating = $state(0);
	let hoveredRating = $state(0);
	let submitting = $state(false);
</script>

<svelte:head>
	<title>{t(locale(), 'reviews.title')}</title>
	<meta name="description" content={t(locale(), 'reviews.metaDescription')} />
	<link rel="canonical" href="https://interstyle.kg/reviews" />
	<meta property="og:type" content="website" />
	<meta property="og:title" content={t(locale(), 'reviews.ogTitle')} />
	<meta property="og:description" content={t(locale(), 'reviews.ogDescription')} />
	<meta property="og:url" content="https://interstyle.kg/reviews" />
	{#if data.recaptchaSiteKey}
		<script src="https://www.google.com/recaptcha/api.js?render={data.recaptchaSiteKey}"></script>
	{/if}
</svelte:head>

<main class="container page">
	<section class="hero compact">
		<p class="tag">{t(locale(), 'reviews.tag')}</p>
		<h1>{t(locale(), 'reviews.heading')}</h1>
		<p>{t(locale(), 'reviews.heroText')}</p>
		<a class="btn ghost" href="/">{t(locale(), 'reviews.backHome')}</a>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>{t(locale(), 'reviews.submitHeading')}</h2>
		</div>

		{#if form?.success}
			<div class="form-message success">
				{t(locale(), 'reviews.successMessage')}
			</div>
		{/if}

		{#if form?.errors?.form}
			<div class="form-message error">{form.errors.form}</div>
		{/if}

		<form
			method="POST"
			use:enhance={async ({ formData, cancel }) => {
				submitting = true;

				if (data.recaptchaSiteKey && typeof (window as any).grecaptcha !== 'undefined') {
					try {
						const rc = (window as any).grecaptcha;
						const token = await new Promise<string>((resolve, reject) => {
							rc.ready(async () => {
								try {
									const t = await rc.execute(data.recaptchaSiteKey, {
										action: 'submit_review'
									});
									resolve(t);
								} catch (e: unknown) {
									reject(e);
								}
							});
						});
						formData.set('captchaToken', token);
					} catch {
						formData.set('captchaToken', '');
					}
				}

				return async ({ update }) => {
					await update();
					submitting = false;
					if (form?.success) {
						selectedRating = 0;
					}
				};
			}}
			class="review-form"
		>
			<div class="form-field">
				<label for="customerName">{t(locale(), 'reviews.nameLabel')}</label>
				<input
					type="text"
					id="customerName"
					name="customerName"
					placeholder={t(locale(), 'reviews.namePlaceholder')}
					minlength="2"
					maxlength="100"
					required
					value={form?.customerName ?? ''}
				/>
				{#if form?.errors?.customerName}
					<p class="field-error">{form.errors.customerName}</p>
				{/if}
			</div>

			<div class="form-field">
				<label for="rating">{t(locale(), 'reviews.ratingLabel')}</label>
				<div class="star-picker" role="radiogroup" aria-label={t(locale(), 'reviews.ratingAriaLabel')}>
					{#each [1, 2, 3, 4, 5] as star}
						<button
							type="button"
							class="star-btn"
							class:active={star <= (hoveredRating || selectedRating)}
							aria-label={`${star} / 5`}
							onmouseenter={() => (hoveredRating = star)}
							onmouseleave={() => (hoveredRating = 0)}
							onclick={() => (selectedRating = star)}
						>
							{star <= (hoveredRating || selectedRating) ? '★' : '☆'}
						</button>
					{/each}
				</div>
				<input type="hidden" id="rating" name="rating" value={selectedRating} />
				{#if form?.errors?.rating}
					<p class="field-error">{form.errors.rating}</p>
				{/if}
			</div>

			<div class="form-field">
				<label for="comment">{t(locale(), 'reviews.commentLabel')}</label>
				<textarea
					id="comment"
					name="comment"
					placeholder={t(locale(), 'reviews.commentPlaceholder')}
					rows="4"
					minlength="5"
					maxlength="2000"
					required>{form?.comment ?? ''}</textarea>
				{#if form?.errors?.comment}
					<p class="field-error">{form.errors.comment}</p>
				{/if}
			</div>

			{#if form?.errors?.captcha}
				<p class="field-error">{form.errors.captcha}</p>
			{/if}

			<button type="submit" class="btn" disabled={submitting}>
				{submitting ? t(locale(), 'reviews.submitting') : t(locale(), 'reviews.submitBtn')}
			</button>
		</form>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>{t(locale(), 'reviews.allHeading')}</h2>
			<p>
				{t(locale(), 'reviews.pageOf', { page: data.reviewsPage.page, total: data.reviewsPage.totalPages })} · {t(locale(), 'reviews.totalCount', { count: data.reviewsPage.totalCount })}
			</p>
		</div>
		<div class="grid reviews">
			{#each data.reviewsPage.items as review}
				<ReviewCard {review} />
			{/each}
		</div>

		<div class="pager" aria-label="Pagination">
			{#if data.reviewsPage.hasPreviousPage}
				<a class="btn ghost" href={prevHref()}>{t(locale(), 'reviews.prev')}</a>
			{:else}
				<span class="btn ghost disabled" aria-disabled="true">{t(locale(), 'reviews.prev')}</span>
			{/if}

			{#if data.reviewsPage.hasNextPage}
				<a class="btn ghost" href={nextHref()}>{t(locale(), 'reviews.next')}</a>
			{:else}
				<span class="btn ghost disabled" aria-disabled="true">{t(locale(), 'reviews.next')}</span>
			{/if}
		</div>
	</section>
</main>
