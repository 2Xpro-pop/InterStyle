<script lang="ts">
	import CurtainCard from '$lib/components/CurtainCard.svelte';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();
</script>

<svelte:head>
	<title>InterStyle - Пошив занавесок от Оша до Кочкор-Ата</title>
	<meta
		name="description"
		content="Индивидуальный пошив занавесок в Кыргызстане: подбор ткани, аккуратный монтаж и расчет цены под ваш интерьер."
	/>
	<link rel="canonical" href="https://interstyle.kg/" />
	<meta property="og:type" content="website" />
	<meta property="og:title" content="InterStyle - Пошив занавесок от Оша до Кочкор-Ата" />
	<meta
		property="og:description"
		content="Индивидуальный пошив занавесок с расчетом цены под ваш проект."
	/>
	<meta property="og:url" content="https://interstyle.kg/" />
</svelte:head>

<main class="container page">
	<section class="hero">
		<p class="tag">InterStyle</p>
		<h1>Вешаем зановески от Оша до Кочкор-Ата</h1>
		<p>
			Индивидуальный пошив, аккуратный монтаж и подбор ткани под ваш интерьер.
		</p>
		<a class="btn" href="/curtains">Смотреть все занавески</a>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>Популярные занавески</h2>
			<a href="/curtains">Весь каталог</a>
		</div>
		<div class="grid cards">
			{#each data.curtains as curtain}
				<CurtainCard {curtain} titleTag="h3" />
			{/each}
		</div>
	</section>

	<section class="section notice">
		<h2>Почему нет фиксированной цены</h2>
		<p>
			Цена рассчитывается индивидуально: размеры окон, выбранная ткань, тип пошива,
			дополнительные декорации и сложность монтажа всегда разные.
		</p>
		<p>
			Чтобы узнать точную стоимость, напишите нам в WhatsApp или Instagram, и мы
			подготовим расчет.
		</p>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>Отзывы</h2>
			<a href="/reviews">Все отзывы</a>
		</div>
		<div class="grid reviews">
			{#each data.reviews as review}
				<article class="review">
					<p class="rating" aria-label={`Оценка ${review.rating} из 5`}>
						{'★'.repeat(review.rating)}{'☆'.repeat(5 - review.rating)}
					</p>
					<p>"{review.comment}"</p>
					<span>{review.customerName}</span>
				</article>
			{/each}
		</div>
	</section>

	<section class="section contacts">
		<h2>Контакты</h2>
		<ul>
			<li><a href="https://wa.me/996700000000" target="_blank" rel="noreferrer">WhatsApp</a></li>
			<li><a href="https://instagram.com/interstyle.kg" target="_blank" rel="noreferrer">Instagram</a></li>
			<li><a href="https://2gis.kg" target="_blank" rel="noreferrer">2GIS</a></li>
		</ul>
	</section>
</main>
