import type { Locale } from './locale';

const translations = {
	ru: {
		// Nav
		'nav.home': 'Главная',
		'nav.curtains': 'Занавески',
		'nav.reviews': 'Отзывы',

		// Footer
		'footer.text': 'InterStyle · Индивидуальный пошив занавесок',

		// Home
		'home.title': 'InterStyle - Пошив занавесок от Оша до Кочкор-Ата',
		'home.metaDescription':
			'Индивидуальный пошив занавесок в Кыргызстане: подбор ткани, аккуратный монтаж и расчет цены под ваш интерьер.',
		'home.ogDescription': 'Индивидуальный пошив занавесок с расчетом цены под ваш проект.',
		'home.heroHeading': 'Вешаем зановески от Оша до Кочкор-Ата',
		'home.heroText': 'Индивидуальный пошив, аккуратный монтаж и подбор ткани под ваш интерьер.',
		'home.heroCta': 'Смотреть все занавески',
		'home.popularHeading': 'Популярные занавески',
		'home.catalogLink': 'Весь каталог',
		'home.pricingHeading': 'Почему нет фиксированной цены',
		'home.pricingText1':
			'Цена рассчитывается индивидуально: размеры окон, выбранная ткань, тип пошива, дополнительные декорации и сложность монтажа всегда разные.',
		'home.pricingText2':
			'Чтобы узнать точную стоимость, напишите нам в WhatsApp или Instagram, и мы подготовим расчет.',
		'home.reviewsHeading': 'Отзывы',
		'home.allReviewsLink': 'Все отзывы',
		'home.contactsHeading': 'Контакты',

		// Curtains catalog
		'curtains.title': 'Каталог занавесок - InterStyle',
		'curtains.metaDescription':
			'Каталог возможных занавесок InterStyle. Выберите стиль и получите индивидуальный расчет стоимости.',
		'curtains.ogDescription':
			'Все варианты занавесок с индивидуальным пошивом и подбором ткани.',
		'curtains.tag': 'Каталог',
		'curtains.heading': 'Все возможные занавески',
		'curtains.heroText':
			'Выберите стиль и напишите нам для индивидуального расчета стоимости.',
		'curtains.backHome': 'Вернуться на главную',

		// Reviews
		'reviews.title': 'Отзывы клиентов - InterStyle',
		'reviews.metaDescription':
			'Отзывы клиентов InterStyle о пошиве и установке занавесок. Реальные оценки и комментарии.',
		'reviews.ogTitle': 'Отзывы клиентов - InterStyle',
		'reviews.ogDescription':
			'Реальные отзывы клиентов о пошиве занавесок и сервисе InterStyle.',
		'reviews.tag': 'Отзывы',
		'reviews.heading': 'Отзывы наших клиентов',
		'reviews.heroText':
			'Публикуем только одобренные отзывы из сервиса InterStyle.Reviews.Api.',
		'reviews.backHome': 'Вернуться на главную',
		'reviews.submitHeading': 'Оставить отзыв',
		'reviews.successMessage':
			'Спасибо за ваш отзыв! Он будет опубликован после модерации.',
		'reviews.nameLabel': 'Ваше имя',
		'reviews.namePlaceholder': 'Как вас зовут?',
		'reviews.ratingLabel': 'Оценка',
		'reviews.ratingAriaLabel': 'Выберите оценку',
		'reviews.commentLabel': 'Комментарий',
		'reviews.commentPlaceholder': 'Расскажите о вашем опыте...',
		'reviews.submitBtn': 'Отправить отзыв',
		'reviews.submitting': 'Отправка...',
		'reviews.allHeading': 'Все отзывы',
		'reviews.pageOf': 'Страница {page} из {total}',
		'reviews.totalCount': 'Всего: {count}',
		'reviews.prev': 'Назад',
		'reviews.next': 'Вперед',

		// Validation
		'validation.nameMin': 'Имя должно содержать минимум 2 символа.',
		'validation.nameMax': 'Имя не должно превышать 100 символов.',
		'validation.ratingRange': 'Оценка должна быть от 1 до 5.',
		'validation.commentMin': 'Комментарий должен содержать минимум 5 символов.',
		'validation.commentMax': 'Комментарий не должен превышать 2000 символов.',
		'validation.captcha': 'Подтвердите, что вы не робот.',
		'validation.submitError': 'Не удалось отправить отзыв. Попробуйте позже.',

		// ReviewCard
		'review.ratingAria': 'Оценка {rating} из 5'
	},
	kg: {
		// Nav
		'nav.home': 'Башкы бет',
		'nav.curtains': 'Парда',
		'nav.reviews': 'Пикирлер',

		// Footer
		'footer.text': 'InterStyle · Жеке тигилген парда',

		// Home
		'home.title': 'InterStyle - Оштон Кочкор-Атага чейин парда тигүү',
		'home.metaDescription':
			'Кыргызстанда жеке парда тигүү: кездеме тандоо, тыкан монтаж жана интерьериңизге баа эсептөө.',
		'home.ogDescription': 'Долбооруңузга баа эсептөө менен жеке парда тигүү.',
		'home.heroHeading': 'Оштон Кочкор-Атага чейин парда тигебиз',
		'home.heroText':
			'Жеке тигүү, тыкан монтаж жана интерьериңизге ылайык кездеме тандоо.',
		'home.heroCta': 'Бардык пардаларды көрүү',
		'home.popularHeading': 'Популярдуу парда',
		'home.catalogLink': 'Толук каталог',
		'home.pricingHeading': 'Эмне үчүн белгиленген баа жок',
		'home.pricingText1':
			'Баа жеке эсептелет: терезенин өлчөмдөрү, тандалган кездеме, тигүү түрү, кошумча жасалгалар жана монтаждын татаалдыгы ар дайым ар башка.',
		'home.pricingText2':
			'Так бааны билүү үчүн, бизге WhatsApp же Instagram аркылуу жазыңыз, биз эсеп даярдайбыз.',
		'home.reviewsHeading': 'Пикирлер',
		'home.allReviewsLink': 'Бардык пикирлер',
		'home.contactsHeading': 'Байланыштар',

		// Curtains catalog
		'curtains.title': 'Парда каталогу - InterStyle',
		'curtains.metaDescription':
			'InterStyle пардаларынын каталогу. Стилди тандап, жеке баа эсебин алыңыз.',
		'curtains.ogDescription':
			'Жеке тигүү жана кездеме тандоо менен пардалардын бардык варианттары.',
		'curtains.tag': 'Каталог',
		'curtains.heading': 'Бардык мүмкүн болгон парда',
		'curtains.heroText':
			'Стилди тандап, жеке баа эсеби үчүн бизге жазыңыз.',
		'curtains.backHome': 'Башкы бетке кайтуу',

		// Reviews
		'reviews.title': 'Кардарлардын пикирлери - InterStyle',
		'reviews.metaDescription':
			'InterStyle кардарларынын парда тигүү жана орнотуу жөнүндөгү пикирлери. Чыныгы баалар жана комментарийлер.',
		'reviews.ogTitle': 'Кардарлардын пикирлери - InterStyle',
		'reviews.ogDescription':
			'Парда тигүү жана InterStyle сервиси жөнүндө чыныгы пикирлер.',
		'reviews.tag': 'Пикирлер',
		'reviews.heading': 'Биздин кардарлардын пикирлери',
		'reviews.heroText':
			'InterStyle.Reviews.Api сервисинен бекитилген пикирлерди гана жарыялайбыз.',
		'reviews.backHome': 'Башкы бетке кайтуу',
		'reviews.submitHeading': 'Пикир калтыруу',
		'reviews.successMessage':
			'Пикириңиз үчүн рахмат! Модерациядан кийин жарыяланат.',
		'reviews.nameLabel': 'Атыңыз',
		'reviews.namePlaceholder': 'Атыңыз ким?',
		'reviews.ratingLabel': 'Баа',
		'reviews.ratingAriaLabel': 'Бааны тандаңыз',
		'reviews.commentLabel': 'Комментарий',
		'reviews.commentPlaceholder': 'Тажрыйбаңыз жөнүндө айтып бериңиз...',
		'reviews.submitBtn': 'Пикир жөнөтүү',
		'reviews.submitting': 'Жөнөтүлүүдө...',
		'reviews.allHeading': 'Бардык пикирлер',
		'reviews.pageOf': 'Барак {page} / {total}',
		'reviews.totalCount': 'Бардыгы: {count}',
		'reviews.prev': 'Артка',
		'reviews.next': 'Алдыга',

		// Validation
		'validation.nameMin': 'Ат кеминде 2 белгиден турушу керек.',
		'validation.nameMax': 'Ат 100 белгиден ашпашы керек.',
		'validation.ratingRange': 'Баа 1ден 5ке чейин болушу керек.',
		'validation.commentMin': 'Комментарий кеминде 5 белгиден турушу керек.',
		'validation.commentMax': 'Комментарий 2000 белгиден ашпашы керек.',
		'validation.captcha': 'Робот эмесиңизди тастыктаңыз.',
		'validation.submitError': 'Пикирди жөнөтүү мүмкүн болгон жок. Кийинчерээк аракеттениңиз.',

		// ReviewCard
		'review.ratingAria': 'Баа {rating} / 5'
	}
} as const;

export type TranslationKey = keyof (typeof translations)['ru'];

export function t(locale: Locale, key: TranslationKey, params?: Record<string, string | number>): string {
	let text: string = translations[locale]?.[key] ?? translations.ru[key] ?? key;
	if (params) {
		for (const [k, v] of Object.entries(params)) {
			text = text.replaceAll(`{${k}}`, String(v));
		}
	}
	return text;
}
