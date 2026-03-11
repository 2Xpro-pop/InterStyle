<script lang="ts">
	import './layout.css';
	import favicon from '$lib/assets/favicon.svg';

	let { children } = $props();
</script>

<svelte:head><link rel="icon" href={favicon} /></svelte:head>

<div class="site-shell">
	<header class="site-header">
		<div class="container nav-row">
			<a class="brand" href="/">InterStyle</a>
			<nav>
				<a href="/">Главная</a>
				<a href="/curtains">Занавески</a>
				<a href="/reviews">Отзывы</a>
			</nav>
		</div>
	</header>

	{@render children()}

	<footer class="site-footer">
		<div class="container">
			<p>InterStyle · Индивидуальный пошив занавесок</p>
		</div>
	</footer>
</div>
