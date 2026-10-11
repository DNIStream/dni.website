---

description: "Task list for the static site rewrite"
---

# Tasks: Static Site Rewrite

**Input**: Design documents from `/specs/001-static-site-rewrite/`

**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Tests**: required. The constitution (Principle VI) makes every behaviour change test-first, so
each story's tests come before its implementation and must fail first.

**Organization**: grouped by user story. Phases 3–8 are delivered and then **the run stops for the
user's local container test** (research R15). Phase 9 (production onboarding) starts only after
that test; Phase 10 (retirement) runs one area at a time, each after its manual test group is
signed off.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: can run in parallel (different files, no dependency on an incomplete task)
- **[Story]**: the user story the task belongs to (US1–US7)

---

## Phase 1: Setup

- [x] T001 Create `package.json` (`"type": "module"`, `engines.node >=24`) with scripts `clean`, `build` (`clean && eleventy && node scripts/optimize-assets.mjs`), `serve` (`eleventy --serve --port 8080`), `test:unit` (`node --test tests/unit/`), `test:build` (`node --test tests/build/`), `test:e2e` (`bash scripts/test-e2e.sh`), `feed:refresh`, `container:up`, `container:down`, `container:stats`; install `@11ty/eleventy`, `fast-xml-parser`, `sanitize-html`, `html-minifier-terser`, `clean-css`, `terser`, `@fontsource-variable/montserrat`, `@playwright/test`, `@axe-core/playwright` as devDependencies; commit `package-lock.json`
- [x] T002 [P] Create `eleventy.config.js`: input `site/`, includes `_includes`, data `_data`, output `dist/`; Nunjucks for `.njk` and Markdown; passthrough `site/assets/` → `dist/assets/` and the Montserrat Latin variable WOFF2 from `node_modules/@fontsource-variable/montserrat/files/` → `dist/assets/fonts/`; global data `env.siteUrl` from `SITE_URL` (default `http://localhost:8080`) and `env.plausibleDomain` from `PLAUSIBLE_DOMAIN` (default empty)
- [x] T003 [P] Add `test-results/` and `playwright-report/` under the existing ignore sections of `.gitignore` (`node_modules/` and `dist/` are already ignored)
- [x] T004 [P] Create `playwright.config.js`: `testDir: tests/e2e`, Chromium only, `baseURL` from `BASE_URL` (default `http://localhost:8089`), 2 workers, 1 retry, trace on first retry, list reporter

---

## Phase 2: Foundational (blocks every story)

### Tests first

- [x] T005 [P] Unit tests for keyword normalising and slugs in `tests/unit/keywords.test.mjs`: trim, lowercase, spaces → `-`; empties dropped; de-duplicated keeping order; slug `c#` → `c-sharp`, `c++` → `c-plus-plus`, other characters outside `[a-z0-9-]` → `-`, runs of `-` collapsed; a slug equal to `page` throws
- [x] T006 [P] Unit tests for paging in `tests/unit/paging.test.mjs`: `PAGE_SIZE = 12`; `total = ceil(count / 12)`, at least 1; 84 items → 7 pages of 12; 13 items → pages of 12 and 1; `href` for page 1 is the root and page n is `<root>page/<n>/`; `first`/`previous` null on page 1, `next`/`last` null on the last page
- [x] T007 [P] Unit tests for show-notes sanitising in `tests/unit/sanitize.test.mjs`: keeps `p ul ol li h2 h3 h4 strong em b i a img br blockquote code pre`; removes `script`, `iframe`, `style` elements, `on*` attributes and `style` attributes; allows only `http`, `https` and `mailto` links; adds `rel="noopener noreferrer"` to external links
- [x] T008 [P] Unit tests for the feed in `tests/unit/feed.test.mjs` against `site/_data/feed-snapshot.xml`: 84 Episodes, newest first, ties in feed order; every `slug` matches `^[a-z0-9-]+$` and is unique (duplicate or mismatch throws); `title` and `published` required; `summary` from `itunes:subtitle`, else first 200 plain-text characters of `description`; `durationSeconds` from `H:MM:SS`, `MM:SS` or seconds, null when absent, `durationLabel` `1h 04m` / `47m`; `image` from `itunes:image`, else the channel image; `audio.url` required, `audio.type` defaults to `audio/mpeg`; keywords normalised; fallback to the snapshot (and a one-line log) when the fetch rejects, exceeds the 15-second timeout, returns non-XML, or yields no items; `FEED_OFFLINE=1` never fetches
- [x] T009 Commit the live feed as `site/_data/feed-snapshot.xml` (fetched from `https://feeds.fireside.fm/dnistream/rss`, 84 items)

### Implementation

- [x] T010 [P] Implement `lib/keywords.js` (`normaliseKeywords`, `keywordSlug`) to pass T005
- [x] T011 [P] Implement `lib/paging.js` (`PAGE_SIZE`, `paginate(items, root)`) to pass T006
- [x] T012 [P] Implement `lib/sanitize.js` (`sanitizeNotes`) with `sanitize-html` to pass T007
- [x] T013 Implement `lib/feed.js` (`loadEpisodes({ offline, fetchImpl, timeoutMs })`: fetch, fallback, parse with `fast-xml-parser`, normalise per data-model.md, sort) to pass T008
- [x] T014 Create `site/_data/episodes.js` exposing `{ source, episodes, latest, keywords, topKeywords (10 by count, ties by name), archivePages, keywordPages }` from `lib/feed.js`, `lib/keywords.js` and `lib/paging.js`; log episode and keyword counts and the source (live or snapshot)
- [x] T015 [P] Create `site/_data/site.json`: `name` "DNI Stream", `description`, `feedUrl` (the public feed), `web3formsKey` "df519f5e-f071-4f08-adce-93be0833cc8c", `copyright`
- [x] T016 [P] Create `site/_data/creators.json` from the old `src/DNI.Web/src/assets/data/creators.json` and add the two creator photos, resized to 320 px WebP, in `site/assets/img/creators/`
- [x] T017 [P] Create `site/_data/platforms.json` (Apple Podcasts, Spotify, Pocket Casts, Overcast, Castro, Castbox, TuneIn; Google Podcasts and Stitcher dropped) with their icons in `site/assets/img/platforms/`, links taken from the old `src/DNI.Web/src/assets/data/discovery-platforms.json` and each checked once by hand to still resolve
- [x] T018 [P] Create `site/_data/social.json` with direct profile URLs (GitHub, X, Twitch, the YouTube channel, the Discord invite, LinkedIn) and inline SVG icon partials in `site/_includes/partials/icons/`
- [x] T019 Create the design system in `site/assets/css/site.css`: tokens in `:root` (dark purple base, DNI orange accent, text, surfaces, type scale, spacing, radii) with a `@media (prefers-color-scheme: light)` override, both meeting WCAG AA contrast; Montserrat `@font-face` with `font-display: swap`; base elements, skip link, focus styles, header, nav, footer, container, card, button, pill, prose, `prefers-reduced-motion`
- [x] T020 Create `site/_includes/layouts/base.njk`, `site/_includes/layouts/page.njk` and partials `head.njk` (title, description, canonical and Open Graph from `env.siteUrl`, RSS alternate link, theme colour, favicon, Plausible script only when `env.plausibleDomain` is set; CSS and JS URLs carry `?v=<content hash>` from a filter in `eleventy.config.js`), `header.njk` (logo, Home, Podcast Archive, Listen / Subscribe, Contact, menu button), `footer.njk` (site links, policy links, social links, copyright) in `site/_includes/partials/`
- [x] T021 Create `site/assets/js/site.js` (deferred, dependency-free): mobile nav toggle with `aria-expanded`, and the artwork fallback that hides failed images (`complete && naturalWidth === 0` at start-up plus a capture-phase `error` listener)
- [x] T022 Create `scripts/optimize-assets.mjs` (minify `dist/**/*.css` with `clean-css` and `dist/**/*.js` with `terser`, failures non-fatal) and add an HTML-minify transform in `eleventy.config.js`
- [ ] T023 [P] Build-output tests in `tests/build/budget.test.mjs`: every page's first-party CSS plus JS under 30 KB minified; no page's static HTML references a host outside those named in the Content-Security-Policy in contracts/container.md; no `iframe` element in any page's static HTML (the managed captcha injects its own frame at runtime, which the policy allows); no `/api/` links
- [ ] T024 [P] E2E container contract tests in `tests/e2e/container.spec.js` per contracts/container.md: `/healthz` → 200 `ok` with `no-store`; security headers including `Content-Security-Policy` on an HTML page, a CSS file and the 404; HTML `no-cache`; long cache on assets; gzip on a text asset; `/api/shows` and `/nope` → 404 with the site's 404 page
- [ ] T025 Create `Dockerfile` (stage `build`: `node:24-alpine`, `npm ci`, copy sources, `ARG SITE_URL PLAUSIBLE_DOMAIN`, `FEED_OFFLINE=1 npm run build`; stage `site`: `alpine:3.22`, `apk add --no-cache nginx`, copy `docker/` config and `dist/`, `USER nginx`, `EXPOSE 8080`, `HEALTHCHECK` on `/healthz`), `Dockerfile.dockerignore`, `docker/nginx.conf` (one worker, pid and temp paths in `/tmp`, listen 8080, `absolute_redirect off`, `try_files $uri $uri/ $uri.html =404`, gzip, caching, `error_page 404 /404.html`, `/healthz`) and `docker/security-headers.conf` (included in every location), plus a minimal `site/404.njk` (permalink `/404.html`) and a placeholder `site/index.njk`, to pass T024
- [ ] T026 Create `scripts/test-e2e.sh`: `FEED_OFFLINE` build of the image, run it as production does (`--read-only --tmpfs /tmp --memory 48m --cpus 0.25 --cap-drop ALL --security-opt no-new-privileges:true -p 8089:8080`), wait for `/healthz`, run Playwright with `BASE_URL=http://localhost:8089`, always remove the container. The image is built with `PLAUSIBLE_DOMAIN` unset, so no test page loads the analytics script; its paths are checked only in the build output (T048)

**Checkpoint**: an empty site builds, the image serves it, and the foundation tests pass.

---

## Phase 3: User Story 1 — Browse and listen to the podcast archive (P1) 🎯 MVP

**Goal**: archive grid, keyword lists and one page per episode, at the current addresses.

**Independent test**: open `/show-archive`, page to page 7, filter by a keyword, open an episode
and play it; repeat with JavaScript disabled.

### Tests first

- [ ] T027 [P] [US1] Build-output tests in `tests/build/archive.test.mjs`: 7 archive pages at `/show-archive/` and `/show-archive/page/2/`…`/7/`; 84 episode pages at `/show/<slug>/`; a page per keyword at `/show-archive/<keyword-slug>/` (including `c-sharp` and `c-plus-plus`) with extra pages where a keyword has more than 12 episodes
- [ ] T028 [P] [US1] E2E tests in `tests/e2e/archive.spec.js`: 12 cards per page, newest first, each with title, artwork, summary, keyword pills, date and duration; pager first/previous/next/last; 1, 2, 3 and 4 columns at 375, 768, 1024 and 1440 px; top-10 keywords with counts; a keyword pill opens that keyword's list and names it; `/show-archive` without a slash answers 200; paging and filtering work with JavaScript disabled
- [ ] T029 [P] [US1] E2E tests in `tests/e2e/episode.spec.js`: `/show/84-debugging` and `/show/the-mvp` (with and without a trailing slash) show title, artwork, date, duration, keywords, an `<audio>` element with the enclosure source, a download link, a link to the episode on the podcast host, and show notes with no `iframe` or `script`; artwork has `loading="lazy"`, `decoding="async"`, width and height, and its frame shows the placeholder; when the artwork request is aborted the placeholder stays and no broken image is shown; the page carries `PodcastEpisode` JSON-LD

### Implementation

- [ ] T030 [P] [US1] Create the placeholder artwork `site/assets/img/episode-placeholder.svg` in the brand colours
- [ ] T031 [P] [US1] Create partials `site/_includes/partials/episode-card.njk` (artwork frame with placeholder background, lazy/async image with width and height, title linking to `/show/<slug>`, summary, keyword pills, date, duration), `site/_includes/partials/pager.njk` and `site/_includes/partials/keyword-pills.njk`
- [ ] T032 [US1] Create `site/show-archive.njk`, paginating `archivePages` to `/show-archive/` and `/show-archive/page/<n>/`, with the top-keywords block
- [ ] T033 [US1] Create `site/show-archive-keyword.njk`, paginating `keywordPages` to `/show-archive/<keyword-slug>/` and its `page/<n>/` pages, with the keyword named in the title and heading
- [ ] T034 [US1] Create `site/_includes/layouts/episode.njk` and `site/show.njk`, paginating `episodes` (size 1) to `/show/<slug>/`, with the audio player, download link, podcast-host link, sanitised notes and `PodcastEpisode` JSON-LD
- [ ] T035 [US1] Add archive grid (1–4 columns, no gaps mid-row), card, pager, pill, artwork frame and episode-page styles to `site/assets/css/site.css`

**Checkpoint**: US1 passes T027–T029 against the image.

---

## Phase 4: User Story 2 — Find out what DNI is and how to follow it (P1)

**Goal**: home page, Listen / Subscribe menu, direct social links.

**Independent test**: open the home page on a phone and a desktop and follow every outbound link.

### Tests first

- [ ] T036 [P] [US2] E2E tests in `tests/e2e/home.spec.js`: introduction, three feature cards (podcast → archive, live stream → Twitch, community → Discord invite), creators; the latest episode's title, artwork (lazy, placeholder) and a link to its page, with no date shown; no countdown and no embedded player
- [ ] T037 [P] [US2] E2E tests in `tests/e2e/navigation.spec.js`: header and footer on every page type; the menu button toggles navigation at 375 px with `aria-expanded`; Listen / Subscribe lists only the platforms in `platforms.json` and none named Google Podcasts or Stitcher; every social link points at its direct profile URL (no `discord.` or `youtube.` subdomain of the old site); navigation works with JavaScript disabled

### Implementation

- [ ] T038 [P] [US2] Create `site/_includes/partials/latest-episode.njk` (title, artwork via the episode-card artwork frame, link; no date)
- [ ] T039 [US2] Create `site/index.njk`: introduction, the three feature cards, the latest episode and the creators from `creators.json`
- [ ] T040 [US2] Add the Listen / Subscribe disclosure menu (works without JavaScript via `<details>`) to `site/_includes/partials/header.njk`
- [ ] T041 [US2] Add home hero, feature-card, creator and Listen / Subscribe styles to `site/assets/css/site.css`

**Checkpoint**: US1 and US2 pass together.

---

## Phase 5: User Story 3 — Contact the team (P2)

**Goal**: a working, protected contact form, plus Discord and social links.

**Independent test**: submit with valid and invalid input, with and without JavaScript; one real
message reaches the mailbox (manual).

### Tests first

- [ ] T042 [US3] E2E tests in `tests/e2e/contact.spec.js` with `https://api.web3forms.com/submit` and the captcha script stubbed via `page.route`, per contracts/contact-form.md: required `name` and `email`, `message` required with `maxlength="5000"`, hidden `access_key`, `subject`, `from_name`; honeypot `botcheck` hidden from people and assistive technology; submit without a captcha token → the "I am human" message and no request; success → status text, form reset; service error → its message, values kept; network failure → the network message, values kept; Send disabled while sending and a second click sends nothing; a plain link to the Discord invite and the social profiles; no `iframe` on the page; the message field shows its 5,000-character limit as visible hint text tied to the field with `aria-describedby`

### Implementation

- [ ] T043 [US3] Create `site/contact.njk` per contracts/contact-form.md (captcha script on this page only; visible 5,000-character limit hint on the message field)
- [ ] T044 [US3] Add the contact-form enhancement to `site/assets/js/site.js` (states and texts as contracts/contact-form.md, in-flight guard)
- [ ] T045 [US3] Add form, field, status and honeypot styles to `site/assets/css/site.css`

---

## Phase 6: User Story 4 — Read the site's policies and FAQ (P3)

**Goal**: FAQ, privacy and cookies, ethics, community guidelines and the 404 page.

**Independent test**: open each page from the footer; request an unknown address.

### Tests first

- [ ] T046 [P] [US4] E2E tests in `tests/e2e/pages.spec.js`: `/faq`, `/privacy`, `/ethics`, `/community-guidelines` each answer 200 with the title in contracts/urls.md and a first heading; the privacy page names Web3Forms, hCaptcha, the podcast host and Plausible, and does not name Google Analytics, reCAPTCHA or the old mail server; the 404 page has a link home and status 404
- [ ] T047 [P] [US4] E2E accessibility tests in `tests/e2e/accessibility.spec.js`: an axe-core scan (WCAG 2.2 A and AA tags) with no violations on home, archive, a keyword list, an episode, contact, privacy and 404, in both dark and light colour schemes; no horizontal scrolling at 320 px; no cookie and no `localStorage` or `sessionStorage` entry is set on any of those pages (FR-024); the captcha script and form service are stubbed so the scan makes no third-party request
- [ ] T048 [P] [US4] Build-output tests in `tests/build/seo.test.mjs`: every page has a unique title and description, a canonical link and Open Graph tags; `sitemap.xml` lists every page in contracts/urls.md; `robots.txt` names the sitemap; every page links the podcast feed as `rel="alternate" type="application/rss+xml"`; a build without `PLAUSIBLE_DOMAIN` emits no analytics script, and a build into a temporary directory with `PLAUSIBLE_DOMAIN=example.com` emits it with `data-api="/stats/api/event"` on every page

### Implementation

- [ ] T049 [P] [US4] Create `site/faq.md`, `site/ethics.md` and `site/community-guidelines.md` with permalinks `/faq/`, `/ethics/`, `/community-guidelines/`, carrying over the text from the old `src/DNI.Web/src/app/components/pages/` FAQ, ethics and community-guidelines components
- [ ] T050 [P] [US4] Create `site/privacy.md` (`/privacy/`), carrying over the old policy and rewriting its third-party section per FR-024: Web3Forms, hCaptcha via Web3Forms, the podcast host for audio and artwork, cookieless Plausible; no cookies or non-essential storage; Google Analytics, reCAPTCHA and the site's own mail server removed
- [ ] T051 [P] [US4] Complete `site/404.njk`: branded content, a link home, `noindex`, excluded from the sitemap
- [ ] T052 [P] [US4] Create `site/sitemap.xml.njk`, `site/robots.txt.njk` and `site/site.webmanifest.njk`, and copy the favicons from the old `src/DNI.Web/src/assets/icons/` into `site/assets/img/icons/`
- [ ] T070 [US4] [derived] E2E tests in `tests/e2e/every-page.spec.js` per constitution Principle VI, over every HTML page listed in `dist/` (about 300): a 2xx response, the title from contracts/urls.md's pattern and a first heading, header navigation and footer present, every first-party image decodes, and no first-party console error or failed first-party request; third-party requests (artwork, captcha) are excluded from the failure checks

---

## Phase 7: User Story 6 — Publish a new episode (P2)

**Goal**: one command refreshes the snapshot; a rebuild publishes the episode.

### Tests first

- [ ] T053 [US6] Unit tests in `tests/unit/feed-refresh.test.mjs`: the refresh writes the snapshot only when the fetched copy parses and holds at least as many items as the current snapshot; otherwise it exits non-zero and leaves the snapshot unchanged; an added item appears first in `loadEpisodes`

### Implementation

- [ ] T054 [US6] Create `scripts/feed-refresh.mjs` (`npm run feed:refresh`) to pass T053

---

## Phase 8: Local container (US5, part 1)

**Goal**: the user can run the production image locally, with production limits, and see its size
and resource use.

- [ ] T055 [US5] Create `compose.yaml`: service `web` built from `Dockerfile` (`target: site`, args `SITE_URL`, `PLAUSIBLE_DOMAIN` defaulting to empty), `read_only: true`, `tmpfs: [/tmp]`, `mem_limit: 48m`, `cpus: 0.25`, `cap_drop: [ALL]`, `security_opt: [no-new-privileges:true]`, ports `${LOCAL_WEB_PORT:-8080}:8080`
- [ ] T056 [US5] Create `scripts/container-stats.sh` (image size in MB, running memory and CPU from `docker stats --no-stream`, user and port, `/healthz` result) and wire `container:up` (`docker compose up -d --build --wait`), `container:down` and `container:stats` in `package.json`
- [ ] T057 [US5] Create `scripts/check-image.sh`, run at the end of `scripts/test-e2e.sh`: fails when the image is 30 MB or more, when memory in use after the E2E run is 48 MB or more, or when the process runs as root
- [ ] T058 [US5] Create `.github/workflows/site.yml`: on pull requests and pushes to `master`, `npm ci`, `test:unit`, `FEED_OFFLINE=1 npm run build`, `test:build`, `test:e2e`; no publishing, no deployment
- [ ] T059 [US5] Add a "New static site" section at the top of `README.md`: prerequisites, `npm run build`, `npm run serve`, `npm run container:up` / `container:stats` / `container:down`, the test commands, `npm run feed:refresh`

**Checkpoint — stop for the user's local test.** Phases 9 and 10 do not start until the user has
tested the site in the local container.

---

## Phase 9: User Story 5 — Production onboarding (P1, after the local test)

- [ ] T060 [US5] Build-output test `tests/build/compose.test.mjs`: `compose.production.yaml` matches contracts/production-compose.md (one service, image `${WEBAPP_IMAGE_WEB}`, loopback port `${WEBAPP_PORT_WEB}:8080`, no `build`, `container_name` or `env_file`; `x-webapp` keys as specified); `.env.example` holds exactly `SITE_URL` and `PLAUSIBLE_DOMAIN`; no other `.env*` file is tracked
- [ ] T061 [US5] Create `compose.production.yaml` per contracts/production-compose.md, with no comments about hosting
- [ ] T062 [US5] Create `.env.example` (`SITE_URL=https://example.com`, `PLAUSIBLE_DOMAIN=example.com`) and replace the `*.env` rule in `.gitignore` with `.env`, `.env.*`, `!.env.example`
- [ ] T063 [US5] Set `VERSION` to `3.0.0` as the single version source, and document the release as a `v3.0.0` git tag in `README.md`

---

## Phase 10: User Story 7 — Retire the old site (P2, each after sign-off)

Each task runs only after the manual test group for what replaced it is signed off, and the PR lists
the deletions grouped by replacement.

- [ ] T064 [US7] Delete the old application and its tests: `src/`, `test/`, `DNI.sln`, `nuget.config`, `run-service-hosts.ps1`, `setup-env-vars.ps1`, `version.ps1`
- [ ] T065 [US7] Delete the deployment material: `deploy.sh`, `docker-compose.yml`, `.dockerignore`, `nginx/` (proxy configs, maintenance page, notes), `.github/workflows/cd.yml`; remove the `docker-username` and `docker-password` lines from `.gitignore`
- [ ] T066 [US7] Delete `.github/workflows/ci.yml` and update `.github/CONTRIBUTING.md` to the new commands
- [ ] T067 [US7] Rewrite `README.md` for the new site only, and update `.claude/worktree-prep.json` (copy `node_modules`, run `npm ci`, ports `LOCAL_WEB_PORT` only, no `dotnet restore`)
- [ ] T068 [US7] Build-output test `tests/build/clean.test.mjs`: no tracked file under the deleted paths; no tracked file carries hosting detail: deploy or proxy server names, IP addresses, server paths, registry names or credentials, TLS certificate paths, or deployment steps (public third-party service URLs the site uses, such as the feed, the form service and listening platforms, are allowed)

---

## Phase 11: Polish

- [ ] T069 Run the quickstart.md scenarios end to end against the local container; run a Lighthouse audit (`npx lighthouse` against the container, mobile profile, using Playwright's Chromium) on one page of each template (home, archive, keyword list, episode, contact, a policy page and the 404 page) as the SC-003 sample; record the scores against SC-002 and SC-003 and the image size and memory figures against SC-006 in the PR description

---

## Dependencies & execution order

- Setup (T001–T004) → Foundational (T005–T026) → US1 → US2 → US3 → US4 → US6 → local container
  (T055–T059) → **user's local test** → US5 onboarding (T060–T063) → US7 retirement (T064–T068,
  each after sign-off) → T069.
- Within a phase, tests come first and must fail before the implementation that follows them.
- `site/assets/css/site.css`, `site/assets/js/site.js` and `site/_includes/partials/header.njk` are
  shared files: tasks touching them run one at a time, in ID order.

## Parallel examples

- Foundational tests: T005, T006, T007, T008 together; then T010, T011, T012 together.
- Data files: T015, T016, T017, T018 together.
- US1 tests: T027, T028, T029 together; then T030 and T031 together.
- US4: T046, T047, T048 together; then T049, T050, T051, T052 together.

## Implementation strategy

MVP is US1 on the foundation: the archive and episode pages served by the image. Each later story
adds to it without breaking the earlier ones, and the whole suite runs against the image at every
checkpoint.
