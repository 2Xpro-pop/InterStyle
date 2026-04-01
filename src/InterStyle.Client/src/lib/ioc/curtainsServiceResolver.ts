import { env } from '$env/dynamic/public';
import { logger } from '$lib/logger';
import { ApiCurtainsService } from '$lib/services/ApiCurtainsService';
import type { ICurtainsService } from '$lib/services/ICurtainsService';
import { MockCurtainsService } from '$lib/services/MockCurtainsService';

const log = logger.child({ component: 'CurtainsService' });

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
				log.warn({ culture }, 'API returned empty result, falling back to mock');
			}
			return curtains.length > 0 ? curtains : this.fallback.getAllCurtains(fetchFn, culture);
		} catch (err) {
			log.error({ error: err instanceof Error ? err.message : String(err), culture }, 'API call failed, using mock fallback');
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
		log.info({ apiUrl }, 'Resolved with API gateway');
		cachedService = new FallbackCurtainsService(new ApiCurtainsService(apiUrl), mockService);
	} else {
		log.warn({}, 'PUBLIC_API_GATEWAY_URL not set, using mock service');
		cachedService = mockService;
	}

	return cachedService;
}
