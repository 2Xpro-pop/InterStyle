import type { Curtain } from '$lib/types/curtain';

export interface ICurtainsService {
	getAllCurtains(fetchFn: typeof fetch, locale?: string): Promise<Curtain[]>;
}
