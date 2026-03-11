<script lang="ts">
	import type { PageData, ActionData } from './$types';
	import { enhance } from '$app/forms';

	let { data, form }: { data: PageData; form: ActionData } = $props();

	const prevHref = () =>
		`/reviews?page=${data.reviewsPage.page - 1}&pageSize=${data.reviewsPage.pageSize}`;
	const nextHref = () =>
		`/reviews?page=${data.reviewsPage.page + 1}&pageSize=${data.reviewsPage.pageSize}`;

	let selectedRating = $state(0);
	let hoveredRating = $state(0);
	let submitting = $state(false);
</script>

<svelte:head>
	<title>Отзывы клиентов - InterStyle</title>
	<meta
		name="description"
		content="Отзывы клиентов InterStyle о пошиве и установке занавесок. Реальные оценки и комментарии."
	/>
	<link rel="canonical" href="https://interstyle.kg/reviews" />
	<meta property="og:type" content="website" />
	<meta property="og:title" content="Отзывы клиентов - InterStyle" />
	<meta
		property="og:description"
		content="Реальные отзывы клиентов о пошиве занавесок и сервисе InterStyle."
	/>
	<meta property="og:url" content="https://interstyle.kg/reviews" />
	{#if data.recaptchaSiteKey}
		<script src="https://www.google.com/recaptcha/api.js?render={data.recaptchaSiteKey}"></script>
	{/if}
</svelte:head>

<main class="container page">
	<section class="hero compact">
		<p class="tag">Отзывы</p>
		<h1>Отзывы наших клиентов</h1>
		<p>Публикуем только одобренные отзывы из сервиса InterStyle.Reviews.Api.</p>
		<a class="btn ghost" href="/">Вернуться на главную</a>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>Оставить отзыв</h2>
		</div>

		{#if form?.success}
			<div class="form-message success">
				Спасибо за ваш отзыв! Он будет опубликован после модерации.
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
				<label for="customerName">Ваше имя</label>
				<input
					type="text"
					id="customerName"
					name="customerName"
					placeholder="Как вас зовут?"
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
				<label for="rating">Оценка</label>
				<div class="star-picker" role="radiogroup" aria-label="Выберите оценку">
					{#each [1, 2, 3, 4, 5] as star}
						<button
							type="button"
							class="star-btn"
							class:active={star <= (hoveredRating || selectedRating)}
							aria-label={`${star} из 5`}
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
				<label for="comment">Комментарий</label>
				<textarea
					id="comment"
					name="comment"
					placeholder="Расскажите о вашем опыте..."
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
				{submitting ? 'Отправка...' : 'Отправить отзыв'}
			</button>
		</form>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>Все отзывы</h2>
			<p>
				Страница {data.reviewsPage.page} из {data.reviewsPage.totalPages} · Всего:
				{data.reviewsPage.totalCount}
			</p>
		</div>
		<div class="grid reviews">
			{#each data.reviewsPage.items as review}
				<article class="review">
					<p class="rating" aria-label={`Оценка ${review.rating} из 5`}>
						{'★'.repeat(review.rating)}{'☆'.repeat(5 - review.rating)}
					</p>
					<p>"{review.comment}"</p>
					<span>{review.customerName}</span>
				</article>
			{/each}
		</div>

		<div class="pager" aria-label="Навигация по страницам отзывов">
			{#if data.reviewsPage.hasPreviousPage}
				<a class="btn ghost" href={prevHref()}>Назад</a>
			{:else}
				<span class="btn ghost disabled" aria-disabled="true">Назад</span>
			{/if}

			{#if data.reviewsPage.hasNextPage}
				<a class="btn ghost" href={nextHref()}>Вперед</a>
			{:else}
				<span class="btn ghost disabled" aria-disabled="true">Вперед</span>
			{/if}
		</div>
	</section>
</main>
