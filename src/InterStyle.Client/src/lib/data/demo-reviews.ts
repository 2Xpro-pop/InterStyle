import type { Review } from '$lib/types/review';

export const demoReviews: Review[] = [
	{
		id: 'r1',
		customerName: 'Айдана, Ош',
		rating: 5,
		comment:
			'Пошили занавески точно по размеру, помогли выбрать ткань и установили за один день. Очень довольны!',
		isApproved: true,
		createdAtUtc: '2026-02-10T08:30:00Z',
		approvedAtUtc: '2026-02-11T09:00:00Z'
	},
	{
		id: 'r2',
		customerName: 'Руслан, Кочкор-Ата',
		rating: 5,
		comment:
			'Нужно было нестандартное окно и ламбрекен. Сделали красиво и аккуратно, все как хотели.',
		isApproved: true,
		createdAtUtc: '2026-02-15T12:45:00Z',
		approvedAtUtc: '2026-02-16T10:20:00Z'
	},
	{
		id: 'r3',
		customerName: 'Нурсултан, Жалал-Абад',
		rating: 4,
		comment:
			'Хорошее качество и вежливая команда. Подобрали вариант под бюджет и интерьер.',
		isApproved: true,
		createdAtUtc: '2026-02-18T14:00:00Z',
		approvedAtUtc: '2026-02-19T09:10:00Z'
	}
];
