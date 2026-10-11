# Research: Static Site Rewrite

All Technical Context unknowns are resolved below. Measurements were taken on 2026-10-11.

## R1. Runtime image

- **Decision**: `alpine:3.22` with Alpine's own `nginx` package (nginx 1.28), running as the
  non-root `nginx` user and listening on 8080. A custom `nginx.conf` replaces the package
  default: one worker, pid and temp paths under `/tmp`, no writes elsewhere.
- **Rationale**: SC-006 caps the image at 30 MB. Measured base sizes: `alpine:3.22` + nginx
  15.8 MB; `nginxinc/nginx-unprivileged:alpine-slim` 29.3 MB, so the site's own files would push it
  over; `nginxinc/nginx-unprivileged:alpine` 90.8 MB. The Alpine build includes the gzip, gzip_static,
  HTTP/2 and real-IP modules. Running as `nginx` on port 8080 needs no added capabilities, which
  meets constitution Principle V ("unprivileged nginx Alpine image ... need no added capabilities").
- **Alternatives considered**: `nginxinc/nginx-unprivileged:alpine-slim` (over budget once the site
  is added); a single-binary static server on `scratch` (smaller still, but it is not nginx, which
  the constitution names); Caddy (about 40 MB).

## R2. Build tool and runtime

- **Decision**: Eleventy 3 with Nunjucks layouts and Markdown pages, on Node 24 (the current LTS,
  and what this machine has). Input directory `site/`, output `dist/`.
- **Rationale**: the constitution names Eleventy. `site/` avoids colliding with the old application
  under `src/`, which stays until sign-off (constitution, Migration and Cleanup Workflow).
- **Alternatives considered**: input under `src/` (collides with the old code until it is deleted).

## R3. Podcast feed

- **Decision**: an Eleventy global data module reads the feed at build time with a 15-second
  timeout, parses it with `fast-xml-parser`, and normalises each item into an Episode
  (data-model.md). When the fetch fails, times out, returns non-XML, or yields no items, the build
  reads the committed snapshot `site/_data/feed-snapshot.xml` instead and logs one line saying so.
  Setting `FEED_OFFLINE=1` skips the network and always uses the snapshot, which keeps tests and
  container builds reproducible. `npm run feed:refresh` overwrites the snapshot from the live feed,
  but only after the new copy parses with at least as many items as the old one.
- **Rationale**: the feed is public, about 560 KB, and holds 84 items, the newest from 2022. The raw
  RSS is stored as the snapshot so live and fallback data go through one parser (FR-012, FR-013).
- **Alternatives considered**: browser-side fetching (rejected by constitution Principle I); a
  normalised JSON snapshot (a second format to keep in step with the parser).

## R4. Feed facts that shape the design

- 84 items. Every item has `content:encoded` show notes, and each contains an embedded `<iframe>`
  player. There is no `<itunes:episode>` element; numbers appear only in some slugs.
- Slugs (the last path segment of `<link>`) all match `^[a-z0-9-]+$`, so `/show/<slug>` keeps every
  current address.
- 206 distinct keywords after normalisation (lowercase, spaces → hyphens). Two contain URL-unsafe
  characters, `c#` and `c++`. The most used are `dev` (24 episodes) and `development` (21).
- Durations are `H:MM:SS` or `MM:SS`.
- Artwork is served from the podcast host's image CDN.

## R5. Keyword addresses

- **Decision**: each keyword gets a URL-safe slug. `#` becomes `sharp` and `+` becomes `plus`, so
  `c#` → `c-sharp` and `c++` → `c-plus-plus`; any other character outside `[a-z0-9-]` becomes `-`,
  and runs of `-` collapse. Keyword lists live at `/show-archive/<keyword-slug>/`, with further pages
  at `/show-archive/<keyword-slug>/page/<n>/`. The full archive pages at `/show-archive/page/<n>/`.
- **Rationale**: the old site put the raw keyword in the path, so `c#` already broke there (it
  became a fragment). No keyword normalises to `page`, so `/show-archive/page/…` cannot collide;
  the build fails if one ever does.

## R6. Show-notes sanitising

- **Decision**: `sanitize-html` at build time with an allow-list: paragraphs, lists, headings,
  emphasis, links (`http`, `https` and `mailto` only, with `rel="noopener noreferrer"` added to
  external links), images, `br`, `blockquote`, `code` and `pre`. Everything else is removed,
  including `script`, `iframe`, `style`, inline event handlers and `style` attributes.
- **Rationale**: the edge case requires unsafe markup to be removed. The embedded players duplicate
  the episode page's own audio player and would load a third-party frame on every episode page.

## R7. Episode artwork and placeholder

- **Decision**: artwork stays on the podcast host (FR-011a). Each artwork `<img>` carries
  `loading="lazy"`, `decoding="async"`, explicit `width`/`height`, and sits in a fixed-ratio frame
  whose CSS background is the site's own placeholder SVG. The placeholder shows from first paint and
  stays visible until the image paints over it. A small script hides any image that fails (checking
  `complete && naturalWidth === 0` at start-up, plus a capture-phase `error` listener), so the
  placeholder stays instead of a broken-image icon.
- **Rationale**: this follows the 2026-10-11 clarifications and the constitution's third-party image
  rule, needs no network request for the placeholder beyond one cached SVG, and degrades without
  JavaScript (only a failed image shows the browser's default).

## R8. Contact form

- **Decision**: follow the reference pattern exactly. A plain `<form>` posts to
  `https://api.web3forms.com/submit` with hidden `access_key`, `subject` and `from_name`; `name`,
  `email` and `message` are required (message `maxlength="5000"`); a visually hidden `botcheck`
  checkbox is the honeypot; the managed hCaptcha is `<div class="h-captcha" data-captcha="true">`
  plus Web3Forms' `client/script.js`, never a raw hCaptcha site key. The enhancement script sends
  `fetch` with `Accept: application/json`, refuses to submit without an `h-captcha-response`,
  disables Send while sending, and reports through an `aria-live` status. On failure the fields keep
  their values. The captcha script loads on the contact page only.
- **Rationale**: FR-015 to FR-017. The same pattern already runs in production elsewhere.

## R9. Analytics

- **Decision**: when the build receives `PLAUSIBLE_DOMAIN` (environment variable, or Docker build
  argument), every page's `<head>` gets
  `<script defer data-domain="…" data-api="/stats/api/event" src="/stats/js/s.js"></script>`.
  Without it, no script is emitted. The container serves nothing under `/stats/`.
- **Rationale**: FR-025. Locally and in tests the domain is unset, so no analytics request is made.

## R10. Fonts and icons

- **Decision**: Montserrat (the brand's current typeface) as a self-hosted variable WOFF2, Latin
  subset, taken from the `@fontsource-variable/montserrat` package at build time, with
  `font-display: swap`. Icons are inline SVG partials (social, platform, menu, play, download).
  Listening-platform logos are small SVGs where available, otherwise the existing PNGs resized.
- **Rationale**: constitution Principle II (self-hosted WOFF2, no icon font).

## R11. Listening platforms

- **Decision**: drop Google Podcasts and Stitcher, both shut down. Keep Apple Podcasts, Spotify,
  Pocket Casts, Overcast, Castro, Castbox and TuneIn, each linking to the show. Each link is checked
  by hand once when the data file is written, and dropped if it no longer resolves.
- **Rationale**: spec Assumptions and FR-003.

## R12. Social links

- **Decision**: link straight to each profile: GitHub, X (Twitter), Twitch, the YouTube channel URL
  (not the old `youtube.` subdomain), the Discord invite (not the old `discord.` subdomain), and
  LinkedIn.
- **Rationale**: FR-004.

## R13. Testing layers

- **Decision**:
  - **Unit** (`node --test`, no extra dependency): feed parsing and normalisation, keyword slugs,
    sanitising, paging maths, the snapshot fallback.
  - **Build checks** (`node --test` over `dist/`): page inventory against the URL contract,
    per-page CSS+JS budget, sitemap completeness, and no forbidden content in the output.
  - **E2E** (Playwright, Chromium): runs against the **production image** started with the
    production settings (read-only root, `/tmp` tmpfs, memory and CPU limits). One spec per page
    type, an axe-core accessibility scan per template, contact-form states with the form service
    stubbed at the network layer, nav toggle, 404 status, headers and caching.
  - **Image checks** (shell script): image size, memory in use after the E2E run, `/healthz`,
    non-root user.
- **Rationale**: constitution Principle VI. Stubbing the form service keeps E2E from sending real
  mail; one real submission is a manual test step.

## R14. Local container workflow

- **Decision**: a development `compose.yaml` builds the production `Dockerfile` and runs it exactly
  as production will: read-only root, `/tmp` tmpfs, `mem_limit: 48m`, `cpus: 0.25`, port
  `${LOCAL_WEB_PORT:-8080}:8080`. `npm run container:up` builds and starts it,
  `npm run container:stats` prints the image size and live memory and CPU use, and
  `npm run container:down` stops it. Build context exclusions live in `Dockerfile.dockerignore`, so
  the old application's `.dockerignore` stays untouched until retirement.
- **Rationale**: the user's priority is testing the real container locally and judging its size and
  resource use before any production onboarding.

## R15. Ordering

- **Decision**: phases run in this order: setup → foundation → the user stories in priority order
  (archive, home, contact, policies, feed refresh) → the local container and its measurements →
  **stop for the user's local test**. Production onboarding (`compose.production.yaml`,
  `.env.example`, removal of deployment material) follows only after that test. Retirement of old
  files follows only after each area's manual test group is signed off.
- **Rationale**: user instruction of 2026-10-11, and the constitution's Migration and Cleanup
  Workflow.
