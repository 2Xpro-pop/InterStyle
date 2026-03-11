import { resolveCurtainsService } from '$lib/ioc/curtainsServiceResolver';
import type { ICurtainsService } from '$lib/services/ICurtainsService';

export const prerender = false;

async function getCatalogCurtains(service: ICurtainsService, fetchFn: typeof fetch) {
	return service.getAllCurtains(fetchFn);
}

export async function load({ fetch }) {
	const curtainsService = resolveCurtainsService();
	const curtains = await getCatalogCurtains(curtainsService, fetch);

	return {
		curtains
	};
}
