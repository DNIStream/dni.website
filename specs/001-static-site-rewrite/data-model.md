# Data Model: Static Site Rewrite

All data is built into the static output. Nothing is stored at runtime.

## Episode

Built from one feed `<item>`. Identity: `slug`, which is unique.

| Field | Source | Rule |
|---|---|---|
| `slug` | last path segment of `<link>`, lowercased | MUST match `^[a-z0-9-]+$`; the build fails on a duplicate or a mismatch |
| `title` | `<title>` | required; trimmed |
| `summary` | `<itunes:subtitle>`, else the first 200 characters of plain-text `<description>` | plain text |
| `notesHtml` | `<content:encoded>`, else `<description>` | sanitised (research R6) |
| `published` | `<pubDate>` | parsed to a date; required |
| `durationSeconds` | `<itunes:duration>` (`H:MM:SS`, `MM:SS` or seconds) | integer, or null when absent |
| `durationLabel` | derived | `1h 04m` or `47m`; empty when null |
| `image` | `<itunes:image href>`, else the channel image | absolute `https` URL, or null |
| `keywords` | `<itunes:keywords>` split on `,` | each trimmed, lowercased, spaces → `-`; empties dropped; order kept; de-duplicated |
| `audio` | `<enclosure url type length>` | `url` is required; `type` defaults to `audio/mpeg` |
| `hostUrl` | `<link>` | the episode on the podcast host |

Order: newest `published` first. Ties keep feed order.

## Keyword

Derived from every Episode's `keywords`.

| Field | Rule |
|---|---|
| `name` | the normalised keyword, shown on pills |
| `slug` | URL-safe form (research R5); unique; MUST NOT equal `page` |
| `episodes` | the Episodes carrying it, newest first |
| `count` | `episodes.length` |

The archive shows the 10 keywords with the highest `count` (ties broken by `name`).

## Page (paging)

Built for the full archive and for each Keyword. `PAGE_SIZE = 12`.

| Field | Rule |
|---|---|
| `number` | 1-based |
| `total` | `ceil(count / 12)`, at least 1 |
| `items` | the 12 (or fewer, on the last page) Episodes for this page |
| `href` | page 1 → the list root; page n → `<root>page/<n>/` |
| `first`, `previous`, `next`, `last` | hrefs, or null when they would point at the current page or past the ends |

## Creator

From `site/_data/creators.json`: `name`, `photo` (site path), `bio`, `links[]` of `{label, url}`.

## Listening platform

From `site/_data/platforms.json`: `name`, `icon` (site path), `url`. Only operating platforms.

## Social profile

From `site/_data/social.json`: `name`, `icon` (partial name), `url` (direct profile URL).

## Feed snapshot

`site/_data/feed-snapshot.xml`: the raw RSS as last refreshed. Replaced only by
`npm run feed:refresh`, and only when the new copy parses and holds at least as many items.

## Site settings

From `site/_data/site.json`: `name`, `description`, `feedUrl`, `web3formsKey` (public),
`copyright`. Two values come from the build environment only, so no hosting domain is committed:
`SITE_URL` (canonical origin for canonical links, Open Graph and the sitemap; defaults to
`http://localhost:8080`) and `PLAUSIBLE_DOMAIN` (analytics; unset means no analytics script).
