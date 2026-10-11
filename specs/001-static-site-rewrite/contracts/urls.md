# Contract: Public addresses

Every address below MUST return 200 with the page described, served by the production image.
Addresses without a trailing slash are answered directly (no redirect to an address carrying the
container's port).

| Address | Page | Title pattern |
|---|---|---|
| `/` | Home | `DNI Stream` |
| `/show-archive` and `/show-archive/` | Archive, page 1 | `Podcast Archive \| DNI Stream` |
| `/show-archive/page/<n>/` (n = 2…7) | Archive, page n | `Podcast Archive, page <n> \| DNI Stream` |
| `/show-archive/<keyword-slug>/` | Keyword list, page 1 | `<keyword> episodes \| DNI Stream` |
| `/show-archive/<keyword-slug>/page/<n>/` | Keyword list, page n | `<keyword> episodes, page <n> \| DNI Stream` |
| `/show/<slug>` and `/show/<slug>/` | Episode (all 84 slugs) | `<episode title> \| Documentation Not Included \| DNI Stream` |
| `/contact` | Contact | `Contact \| DNI Stream` |
| `/faq` | FAQ | `FAQ \| DNI Stream` |
| `/privacy` | Privacy and cookie policy | `Privacy & Cookies \| DNI Stream` |
| `/ethics` | Code of ethics | `Code of Ethics \| DNI Stream` |
| `/community-guidelines` | Community guidelines | `Community Guidelines \| DNI Stream` |
| `/sitemap.xml` | Sitemap listing every page above | — |
| `/robots.txt` | Allows all; points at the sitemap | — |
| `/site.webmanifest` | Web app manifest | — |
| `/healthz` | `ok`, 200, never cached | — |

- Any other address, including `/api/...` → the 404 page with status **404**.
- `/page/1/` forms are not published; page 1 lives at the list root.
- Every page carries `<link rel="alternate" type="application/rss+xml">` to the podcast feed and a
  canonical `<link>` built from the `SITE_URL` build setting.
