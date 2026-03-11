<script lang="ts">
	import type { Review } from '$lib/types/review';

	let { review }: { review: Review } = $props();
</script>

<article class="review">
	<p class="rating" aria-label={`Оценка ${review.rating} из 5`}>
		{'★'.repeat(review.rating)}{'☆'.repeat(5 - review.rating)}
	</p>
	<p>"{review.comment}"</p>
	<span>{review.customerName}</span>
</article>
