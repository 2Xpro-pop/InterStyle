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

	async getAllCurtains(fetchFn: typeof fetch) {
		try {
			const curtains = await this.primary.getAllCurtains(fetchFn);
			return curtains.length > 0 ? curtains : this.fallback.getAllCurtains(fetchFn);
		} catch {
			return this.fallback.getAllCurtains(fetchFn);
		}
	}
}

export function resolveCurtainsService(): ICurtainsService {
	if (cachedService) {
		return cachedService;
	}

	const apiUrl = env.PUBLIC_CURTAINS_API_URL;
	const mockService = new MockCurtainsService();
	cachedService = apiUrl
		? new FallbackCurtainsService(new ApiCurtainsService(apiUrl), mockService)
		: mockService;

	return cachedService;
}
