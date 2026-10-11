# Quickstart: Static Site Rewrite

Validation guide. Commands run from the repository root.

## Prerequisites

Node 24 with npm, and Docker with Compose. One-time: `npm ci` and `npx playwright install chromium`.

## 1. Build and preview

```bash
npm run build          # dist/ from the live feed, falling back to the snapshot
npm run serve          # dev server with live reload on http://localhost:8080
```

Expected: the build prints the number of episodes (84) and keywords, and says whether it used the
live feed or the snapshot. `FEED_OFFLINE=1 npm run build` always uses the snapshot.

## 2. Run the production container locally

```bash
npm run container:up     # builds the image, starts it read-only with production limits
npm run container:stats  # image size, memory and CPU in use
npm run container:down
```

Expected: the site answers on `http://localhost:${LOCAL_WEB_PORT:-8080}`, `/healthz` returns `ok`,
the image is under 30 MB and memory in use is under 48 MB (contracts/container.md).

## 3. Automated checks

```bash
npm run test:unit        # feed, keywords, sanitising, paging
npm run test:build       # page inventory, budgets, sitemap, forbidden content (after a build)
npm run test:e2e         # Playwright + axe against the production image (builds and starts it)
```

Expected: all green. `test:e2e` stubs the form service, so it sends no mail.

## 4. Scenarios to see by hand

| Scenario | Where | Expected |
|---|---|---|
| Archive grid | `/show-archive` at 375, 768, 1024 and 1440 px wide | 1, 2, 3 and 4 columns; 12 cards; pager to 7 pages |
| Keyword list | select a keyword pill | only that keyword's episodes, paged, keyword named |
| Episode | `/show/84-debugging` | artwork (placeholder first), date, duration, player plays, notes have no embedded player |
| Old address | `/show/the-mvp` | the episode page |
| Home | `/` | three cards, creators, latest episode with no date, no countdown or live player |
| Theme | switch the OS between dark and light | the site follows |
| Contact | `/contact` | one real message reaches the team mailbox; a missing field is reported; no Discord widget |
| No JavaScript | disable JavaScript, browse the archive and send the form | paging and filtering work; the form posts |
| 404 | `/nope` and `/api/shows` | branded 404 page, status 404 |

## 5. Refresh the feed (new episode)

```bash
npm run feed:refresh     # updates site/_data/feed-snapshot.xml from the live feed
npm run build
```

Expected: a new episode appears first in the archive and on the home page, with its own page.
