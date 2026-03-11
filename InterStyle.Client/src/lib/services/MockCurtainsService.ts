import { demoCurtains } from '$lib/data/demo-curtains';
import type { Curtain } from '$lib/types/curtain';
import type { ICurtainsService } from '$lib/services/ICurtainsService';

export class MockCurtainsService implements ICurtainsService {
	async getAllCurtains(_: typeof fetch): Promise<Curtain[]> {
		return demoCurtains;
	}
}
