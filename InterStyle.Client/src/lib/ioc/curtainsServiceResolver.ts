import { env } from '$env/dynamic/public';
import { ApiCurtainsService } from '$lib/services/ApiCurtainsService';
import type { ICurtainsService } from '$lib/services/ICurtainsService';
import { MockCurtainsService } from '$lib/services/MockCurtainsService';

let cachedService: ICurtainsService | null = null;

class FallbackCurtainsService implements ICurtainsService {
	constructor(
		private readonly primary: ICurtainsService,
		private readonly fallback: ICurtainsService
	) {}

	async getAllCurtains(fetchFn: typeof fetch, culture?: string) {
		try {
			const curtains = await this.primary.getAllCurtains(fetchFn, culture);
			if (curtains.length === 0) {
				console.warn('[CurtainsService] API returned empty, falling back to mock');
			}
			return curtains.length > 0 ? curtains : this.fallback.getAllCurtains(fetchFn, culture);
		} catch (err) {
			console.error('[CurtainsService] API failed, using mock:', err instanceof Error ? err.message : err);
			return this.fallback.getAllCurtains(fetchFn, culture);
		}
	}
}

export function resolveCurtainsService(): ICurtainsService {
	if (cachedService) {
		return cachedService;
	}

	const apiUrl = env.PUBLIC_API_GATEWAY_URL;
	const mockService = new MockCurtainsService();

	if (apiUrl) {
		console.log(`[CurtainsService] Using API gateway: ${apiUrl}`);
		cachedService = new FallbackCurtainsService(new ApiCurtainsService(apiUrl), mockService);
	} else {
		console.warn('[CurtainsService] PUBLIC_API_GATEWAY_URL not set, using mock');
		cachedService = mockService;
	}

	return cachedService;
}
