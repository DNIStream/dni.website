# Implementation Plan: Static Site Rewrite

**Branch**: `feature/S001-static-site-rewrite` | **Date**: 2026-10-11 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-static-site-rewrite/spec.md`

## Summary

Replace the Angular 10 and .NET Core 3.1 site with a static Eleventy site, served by one small
Alpine nginx container. The podcast archive, episode pages and the home page's latest episode are
generated at build time from the public feed, falling back to a committed snapshot. The contact
form posts to Web3Forms. The site gets a dark-first, responsive redesign in the DNI brand.

Order of delivery follows the user's instruction: build the site and run it **locally in the
production container first**, measured for size and memory, and stop for the user's test.
Production onboarding comes after that test, and old files are deleted only after sign-off.

## Technical Context

**Language/Version**: JavaScript (ES modules) on Node 24 for the build; HTML, CSS and vanilla
JavaScript in the browser

**Primary Dependencies**: Eleventy 3 (Nunjucks, Markdown); `fast-xml-parser`; `sanitize-html`;
`html-minifier-terser`, `clean-css` and `terser` for minifying; `@fontsource-variable/montserrat`
for the font file. Dev only: `@playwright/test`, `@axe-core/playwright`. All build-time; nothing
ships in the image but static files.

**Storage**: none at runtime. Build inputs are files: `site/_data/*.json` and
`site/_data/feed-snapshot.xml`

**Testing**: `node --test` for unit and build-output checks; Playwright (Chromium) with axe-core for
E2E against the production image; a shell check for image size, memory and user

**Target Platform**: Linux container (`alpine:3.22` + nginx 1.28, non-root, port 8080); evergreen
browsers on phones and desktops

**Project Type**: static website

**Performance Goals**: main content within 1.5 s on a mid-range phone on fast 4G (SC-002);
performance score of 95 or more for the site's own resources (SC-003)

**Constraints**: image under 30 MB and memory under 48 MB (SC-006); 30 KB of first-party CSS+JS per
page, minified (constitution II); WCAG 2.2 AA; works without JavaScript; read-only root filesystem

**Scale/Scope**: about 300 pages: 11 fixed pages, 84 episodes, 7 archive pages, 206 keyword lists
(about 215 pages including their extra pages)

## Constitution Check

*GATE: checked before Phase 0 and again after Phase 1. Both pass.*

| Principle | How the plan meets it |
|---|---|
| I. Static by Default | No server code. Feed data fetched at build (R3). The only runtime third-party call is the form post with its managed captcha, on the contact page only (R8). Plausible is loaded and reports through the site's own `/stats/` paths, so the browser makes no third-party call for it (R9). Every page renders without JavaScript |
| II. Lightweight and Fast | No framework, CSS framework, jQuery or icon font. Per-page CSS+JS budget enforced by `test:build`. Self-hosted WOFF2. Third-party artwork lazy, async, space reserved (R7) |
| III. Template-Driven Content | One base layout and one set of partials. Lists generated from feed and data files. A new episode needs `feed:refresh` and a rebuild only. Old addresses kept (contracts/urls.md) |
| IV. Responsive, Accessible, Modern | Mobile-first grid from 320 px; tokens in one `:root` block with a `prefers-color-scheme: light` override; axe scan per template; title, description, OG and sitemap per page |
| V. One Lightweight Container | Alpine + nginx, non-root, 8080, `/healthz`, read-only with `/tmp` tmpfs, no added capabilities (R1, contracts/container.md). `compose.production.yaml` and `.env.example` per contracts/production-compose.md. No hosting detail in any file. New CI builds and tests only |
| VI. Verified by Test | Test-first per task; unit, build-output and E2E layers (R13); E2E runs against the production image |
| Migration and Cleanup | New site built alongside the old one (`site/` beside `src/`). Old files deleted only after sign-off, listed in the PR |

One justified deviation, recorded under Complexity Tracking.

## Project Structure

### Documentation (this feature)

```text
specs/001-static-site-rewrite/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── urls.md
│   ├── container.md
│   ├── contact-form.md
│   └── production-compose.md
└── tasks.md
```

### Source Code (repository root)

```text
package.json, package-lock.json
eleventy.config.js            # input site/, output dist/, filters, collections, minify transform
lib/
├── feed.js                   # fetch with timeout, snapshot fallback, parse, normalise
├── keywords.js               # normalise and slug keywords
├── paging.js                 # page maths and pager hrefs
└── sanitize.js               # show-notes allow-list
scripts/
├── feed-refresh.mjs          # npm run feed:refresh
├── optimize-assets.mjs       # minify CSS and JS in dist/
└── container-stats.sh        # npm run container:stats
site/
├── _data/                    # site.json, creators.json, platforms.json, social.json,
│                             # episodes.js, feed-snapshot.xml
├── _includes/
│   ├── layouts/              # base.njk, page.njk, episode.njk
│   └── partials/             # head, header, footer, episode-card, pager, keyword-pills,
│                             # latest-episode, icons/*.njk
├── assets/
│   ├── css/site.css
│   ├── js/site.js            # nav toggle, image fallback, contact form
│   ├── img/                  # logo, placeholder, creators, platforms, favicons
│   └── fonts/                # Montserrat variable WOFF2 (copied at build)
├── index.njk                 # home
├── show-archive.njk          # archive pages (paginated)
├── show-archive-keyword.njk  # keyword list pages (paginated)
├── show.njk                  # one page per episode
├── contact.njk
├── faq.md, privacy.md, ethics.md, community-guidelines.md
├── 404.njk
└── sitemap.xml.njk, robots.txt.njk, site.webmanifest.njk
tests/
├── unit/                     # node --test
├── build/                    # node --test over dist/
└── e2e/                      # Playwright specs
playwright.config.js
docker/nginx.conf, docker/security-headers.conf
Dockerfile, Dockerfile.dockerignore
compose.yaml                  # local: production image with production limits
.github/workflows/site.yml    # build and test only
```

Added after the user's local test: `compose.production.yaml`, `.env.example`. Removed after
sign-off: `src/`, `test/`, `DNI.sln`, `nuget.config`, `run-service-hosts.ps1`,
`setup-env-vars.ps1`, `version.ps1`, `deploy.sh`, `docker-compose.yml`, `.dockerignore`, `nginx/`,
`.github/workflows/ci.yml`, `.github/workflows/cd.yml`.

**Structure Decision**: one static-site project at the repository root, with Eleventy's input in
`site/` so the new site can live beside the old `src/` until retirement.

## Delivery phases

1. **Setup**: package, Eleventy config, test runners, `.gitignore` additions.
2. **Foundation**: feed pipeline with fallback, data files, base layout and partials, design
   tokens and CSS system, the image and its nginx config. Blocks every story.
3. **US1** archive and episodes → **US2** home and navigation → **US3** contact → **US4** policies
   and 404 → **US6** feed refresh.
4. **Local container**: `compose.yaml`, `container:*` scripts, size and memory check, the
   `site.yml` workflow. **The run stops here for the user's local test.**
5. **US5 production onboarding**: after the user's local test.
6. **US7 retirement**: one task per area, each after its manual test group is signed off.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Principle IV: the 404 page and `/healthz` are not in the sitemap | A sitemap lists pages meant to be indexed; the 404 page is `noindex` and `/healthz` is not a page | Listing them would ask search engines to index an error page and a health check |
