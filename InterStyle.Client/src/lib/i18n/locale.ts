export type Locale = 'ru' | 'kg';

export const defaultLocale: Locale = 'ru';

export const localeNames: Record<Locale, string> = {
	ru: 'Рус',
	kg: 'Кыр'
};

export const cultureCodes: Record<Locale, string> = {
	ru: 'ru-RU',
	kg: 'kg-KG'
};

export function isLocale(value: string): value is Locale {
	return value === 'ru' || value === 'kg';
}
