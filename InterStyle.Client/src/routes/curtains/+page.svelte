<script lang="ts">
	import CurtainCard from '$lib/components/CurtainCard.svelte';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();
</script>

<svelte:head>
	<title>Каталог занавесок - InterStyle</title>
	<meta
		name="description"
		content="Каталог возможных занавесок InterStyle. Выберите стиль и получите индивидуальный расчет стоимости."
	/>
	<link rel="canonical" href="https://interstyle.kg/curtains" />
	<meta property="og:type" content="website" />
	<meta property="og:title" content="Каталог занавесок - InterStyle" />
	<meta
		property="og:description"
		content="Все варианты занавесок с индивидуальным пошивом и подбором ткани."
	/>
	<meta property="og:url" content="https://interstyle.kg/curtains" />
</svelte:head>

<main class="container page">
	<section class="hero compact">
		<p class="tag">Каталог</p>
		<h1>Все возможные занавески</h1>
		<p>Выберите стиль и напишите нам для индивидуального расчета стоимости.</p>
		<a class="btn ghost" href="/">Вернуться на главную</a>
	</section>

	<section class="section">
		<div class="grid cards">
			{#each data.curtains as curtain}
				<CurtainCard {curtain} titleTag="h2" />
			{/each}
		</div>
	</section>
</main>
