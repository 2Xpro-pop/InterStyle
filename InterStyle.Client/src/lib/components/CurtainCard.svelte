<script lang="ts">
	import type { Curtain } from '$lib/types/curtain';

	let { curtain, titleTag = 'h3' }: { curtain: Curtain; titleTag?: 'h2' | 'h3' } = $props();
</script>

<article class="card">
	<div class="card-media">
		<img class="picture" src={curtain.pictureUrl} alt={curtain.name} loading="lazy" />
		<img class="preview" src={curtain.previewUrl} alt="" loading="lazy" aria-hidden="true" />
	</div>
	<div class="card-body">
		<svelte:element this={titleTag}>{curtain.name}</svelte:element>
		<p>{curtain.description}</p>
	</div>
</article>
