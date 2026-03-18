const SITE = 'https://interstyle.kg';

const pages = ['/', '/curtains', '/reviews'];

export function GET() {
	const urls = pages
		.map(
			(path) => `
  <url>
    <loc>${SITE}${path}</loc>
    <xhtml:link rel="alternate" hreflang="ru" href="${SITE}${path}" />
    <xhtml:link rel="alternate" hreflang="ky" href="${SITE}${path}?lang=kg" />
    <xhtml:link rel="alternate" hreflang="x-default" href="${SITE}${path}" />
  </url>`
		)
		.join('');

	const xml = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9"
        xmlns:xhtml="http://www.w3.org/1999/xhtml">${urls}
</urlset>`;

	return new Response(xml.trim(), {
		headers: { 'Content-Type': 'application/xml' }
	});
}
