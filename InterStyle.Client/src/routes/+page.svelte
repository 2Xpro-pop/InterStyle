<script lang="ts">
	import CurtainCard from '$lib/components/CurtainCard.svelte';
	import ReviewCard from '$lib/components/ReviewCard.svelte';
	import { t } from '$lib/i18n/translations';
	import { page } from '$app/state';
	import type { Locale } from '$lib/i18n/locale';
	import type { PageData } from './$types';

	let { data }: { data: PageData } = $props();

	const locale = () => (page.data as { locale: Locale }).locale;
</script>

<svelte:head>
	<title>{t(locale(), 'home.title')}</title>
	<meta name="description" content={t(locale(), 'home.metaDescription')} />
	<link rel="canonical" href="https://interstyle.kg/" />
	<meta property="og:type" content="website" />
	<meta property="og:title" content={t(locale(), 'home.title')} />
	<meta property="og:description" content={t(locale(), 'home.ogDescription')} />
	<meta property="og:url" content="https://interstyle.kg/" />
	{@html `<script type="application/ld+json">${JSON.stringify({
		"@context": "https://schema.org",
		"@type": "LocalBusiness",
		"name": "InterStyle",
		"description": t(locale(), 'home.metaDescription'),
		"url": "https://interstyle.kg",
		"image": "https://interstyle.kg/og-image.jpg",
		"address": {
			"@type": "PostalAddress",
			"addressLocality": "Ош",
			"addressCountry": "KG"
		},
		"geo": {
			"@type": "GeoCoordinates",
			"latitude": 40.5283,
			"longitude": 72.7985
		},
		"telephone": "+996550123456",
		"sameAs": [
			"https://www.instagram.com/interstyle.kg/",
			"https://2gis.kg/osh/search/InterStyle"
		]
	})}</script>`}
</svelte:head>

<main class="container page">
	<section class="hero">
		<p class="tag">InterStyle</p>
		<h1>{t(locale(), 'home.heroHeading')}</h1>
		<p>{t(locale(), 'home.heroText')}</p>
		<a class="btn" href="/curtains">{t(locale(), 'home.heroCta')}</a>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>{t(locale(), 'home.popularHeading')}</h2>
			<a href="/curtains">{t(locale(), 'home.catalogLink')}</a>
		</div>
		<div class="grid cards">
			{#each data.curtains as curtain}
				<CurtainCard {curtain} titleTag="h3" />
			{/each}
		</div>
	</section>

	<section class="section notice">
		<h2>{t(locale(), 'home.pricingHeading')}</h2>
		<p>{t(locale(), 'home.pricingText1')}</p>
		<p>{t(locale(), 'home.pricingText2')}</p>
	</section>

	<section class="section">
		<div class="section-head">
			<h2>{t(locale(), 'home.reviewsHeading')}</h2>
			<a href="/reviews">{t(locale(), 'home.allReviewsLink')}</a>
		</div>
		<div class="grid reviews">
			{#each data.reviews as review}
				<ReviewCard {review} />
			{/each}
		</div>
	</section>

	<section class="section contacts">
		<h2>{t(locale(), 'home.contactsHeading')}</h2>
		<ul>
			<li><a href="https://wa.me/996700000000" target="_blank" rel="noreferrer">WhatsApp</a></li>
			<li><a href="https://instagram.com/interstyle.kg" target="_blank" rel="noreferrer">Instagram</a></li>
			<li><a href="https://2gis.kg" target="_blank" rel="noreferrer">2GIS</a></li>
		</ul>
	</section>
</main>
