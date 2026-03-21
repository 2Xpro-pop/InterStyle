import type { ICurtainsService } from '$lib/services/ICurtainsService';
import { logger } from '$lib/logger';
import type { Curtain } from '$lib/types/curtain';

const log = logger.child({ component: 'CurtainsApi' });

interface CurtainApiDto {
	id?: string;
	name?: string;
	description?: string;
	pictureUrl?: string;
	previewUrl?: string;
}

function mapApiCurtain(item: CurtainApiDto, index: number): Curtain {
	return {
		id: item.id ?? `api-${index + 1}`,
		name: item.name ?? `Занавеска ${index + 1}`,
		description: item.description ?? 'Индивидуальный пошив под ваш интерьер.',
		pictureUrl:
			item.pictureUrl ??
			item.previewUrl ??
			'https://images.unsplash.com/photo-1616627561950-9f746e330187?auto=format&fit=crop&w=1200&q=80',
		previewUrl:
			item.previewUrl ??
			item.pictureUrl ??
			'https://images.unsplash.com/photo-1616627561950-9f746e330187?auto=format&fit=crop&w=1200&q=80'
	};
}

export class ApiCurtainsService implements ICurtainsService {
	constructor(private readonly baseUrl: string) {}

	async getAllCurtains(fetchFn: typeof fetch, locale?: string): Promise<Curtain[]> {
		const url = new URL(`${this.baseUrl}/api/curtains`);
		url.searchParams.set('api-version', '1.0');
		if (locale) {
			url.searchParams.set('locale', locale);
		}
		log.info({ url: url.toString(), locale }, 'Fetching curtains');
		const response = await fetchFn(url.toString());
		if (!response.ok) {
			log.error({ url: url.toString(), status: response.status, statusText: response.statusText }, 'Curtains API request failed');
			throw new Error('Curtains API is unavailable');
		}

		const payload = (await response.json()) as CurtainApiDto[];
		const itemCount = Array.isArray(payload) ? payload.length : 0;
		log.info({ itemCount }, 'Curtains fetched successfully');
		if (!Array.isArray(payload) || payload.length === 0) {
			return [];
		}

		return payload.map(mapApiCurtain);
	}
}
