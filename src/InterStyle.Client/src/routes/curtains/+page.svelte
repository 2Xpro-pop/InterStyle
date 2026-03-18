<script lang="ts">
	import CurtainCard from '$lib/components/CurtainCard.svelte';
	import { t } from '$lib/i18n/translations';
	import { page } from '$app/state';
	import type { Locale } from '$lib/i18n/locale';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	const locale = () => (page.data as { locale: Locale }).locale;
</script>

<svelte:head>
	<title>{t(locale(), 'curtains.title')}</title>
	<meta name="description" content={t(locale(), 'curtains.metaDescription')} />
	<link rel="canonical" href="https://interstyle.kg/curtains" />
	<meta property="og:type" content="website" />
	<meta property="og:title" content={t(locale(), 'curtains.title')} />
	<meta property="og:description" content={t(locale(), 'curtains.ogDescription')} />
	<meta property="og:url" content="https://interstyle.kg/curtains" />
</svelte:head>

<main class="container page">
	<section class="hero compact">
		<p class="tag">{t(locale(), 'curtains.tag')}</p>
		<h1>{t(locale(), 'curtains.heading')}</h1>
		<p>{t(locale(), 'curtains.heroText')}</p>
		<a class="btn ghost" href="/">{t(locale(), 'curtains.backHome')}</a>
	</section>

	<section class="section">
		<div class="grid cards">
			{#each data.curtains as curtain}
				<CurtainCard {curtain} titleTag="h2" />
			{/each}
		</div>
	</section>
</main>
