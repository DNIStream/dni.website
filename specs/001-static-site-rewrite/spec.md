# Feature Specification: Static Site Rewrite

**Feature Branch**: `feature/S001-static-site-rewrite`

**Created**: 2026-10-10

**Status**: Draft

**Input**: User description: "Static rewrite of the DNI Stream website, plus its deployment
onboarding. Replace the current site with a static site in one lightweight container: home,
paged podcast archive with keyword filter, one page per episode, contact (form with managed
captcha and honeypot, Discord widget, social links), FAQ, privacy and cookies, ethics, community
guidelines, 404, a Listen/Subscribe menu. Episodes come from the public Fireside feed at build
time, with a committed snapshot fallback. Modern responsive redesign keeping the DNI brand.
Onboarding: the production composition, the production image and `.env.example`, and removal of
all deploy, host, credential and CD material. Obsolete files are deleted only after the user
confirms their replacement works."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Browse and listen to the podcast archive (Priority: P1)

A visitor opens the podcast archive, pages through every episode newest first, narrows the list
to a topic keyword, opens an episode, reads its show notes and plays it in the page.

**Why this priority**: the archive is the reason the site exists; 84 episodes are its content.

**Independent Test**: build the site from the feed snapshot, open the archive, page to the last
page, filter by a keyword, open an episode and start playback.

**Acceptance Scenarios**:

1. **Given** the archive, **When** a visitor opens it, **Then** they see the newest episodes
   first, each with title, image, summary, keywords, publish date and duration, and controls to
   reach every other page of the list.
2. **Given** an episode in the list, **When** the visitor selects it, **Then** its page shows the
   title, image, date, duration, keywords, an in-page audio player, a download link, a link to the
   episode on the podcast host, and the full show notes.
3. **Given** a keyword pill, **When** the visitor selects it, **Then** the list shows only the
   episodes carrying that keyword, paged the same way, and states the keyword being shown.
4. **Given** an episode address from the current site (for example `/show/84-debugging`),
   **When** it is requested, **Then** that episode's page is served at the same address.
5. **Given** a visitor with JavaScript disabled, **When** they browse, page and filter the
   archive, **Then** every step still works.

---

### User Story 2 - Find out what DNI is and how to follow it (Priority: P1)

A first-time visitor lands on the home page, understands what DNI Stream is, meets its creators,
and finds where to listen, watch and join the community.

**Why this priority**: the home page and the Listen/Subscribe menu are how new listeners arrive
and convert.

**Independent Test**: open the home page on a phone and a desktop and follow every outbound link.

**Acceptance Scenarios**:

1. **Given** the home page, **When** it loads, **Then** it shows the brand, an introduction, the
   three feature cards (the podcast, the live development stream, the community) and the
   creators, each linking to their destinations.
2. **Given** any page, **When** the visitor opens Listen / Subscribe, **Then** they see each
   podcast platform that still operates, each opening the show on that platform.
3. **Given** any page, **When** the visitor uses the social links, **Then** each opens the DNI
   profile directly, without depending on an intermediate redirect.
4. **Given** the home page, **When** it loads, **Then** it shows no next-show countdown and no
   embedded live player; the live development stream is reached through its feature card.
5. **Given** the home page, **When** it loads, **Then** it shows the latest episode's title and
   artwork, with a link to its episode page, and shows no date for it.

---

### User Story 3 - Contact the team (Priority: P2)

A visitor sends the team a message from the contact page, or chooses Discord or a social channel
instead.

**Why this priority**: the only inbound channel the site offers; today's endpoint is unprotected.

**Independent Test**: submit the form with valid and invalid input, with and without JavaScript,
and confirm the message arrives by email.

**Acceptance Scenarios**:

1. **Given** the contact page, **When** a visitor completes name, email and message and passes
   the human check, **Then** the message is sent, the form clears, and an on-page message confirms
   it.
2. **Given** the form, **When** a required field is missing or the email is malformed, **Then**
   the visitor is told which field to fix and nothing is sent.
3. **Given** the form, **When** the visitor submits without passing the human check, **Then**
   they are asked to complete it and nothing is sent.
4. **Given** the form service fails or the network drops, **When** the visitor submits, **Then**
   they see that it failed and can retry, and what they typed is kept.
5. **Given** an automated submission that fills the hidden honeypot field, **When** it is
   submitted, **Then** no message reaches the team.
6. **Given** JavaScript is disabled, **When** the visitor submits, **Then** the form still posts
   and the visitor sees the outcome.
7. **Given** the contact page, **When** a visitor prefers Discord or social media, **Then** the
   Discord invite link and each social profile are reachable from the same page, with no
   embedded Discord widget.

---

### User Story 4 - Read the site's policies and FAQ (Priority: P3)

A visitor reads the FAQ, the privacy and cookie policy, the code of ethics, or the community
guidelines.

**Why this priority**: required content, but rarely visited.

**Independent Test**: open each page from the footer and check its content is complete and current.

**Acceptance Scenarios**:

1. **Given** the footer, **When** the visitor selects any policy or FAQ link, **Then** the page
   opens at its current address with its content carried over.
2. **Given** the privacy and cookie policy, **When** it is read, **Then** it names every third
   party the site now contacts (form service, captcha, podcast media host, any font or
   analytics provider) and no longer names services the site has dropped.
3. **Given** an address that does not exist, **When** it is requested, **Then** a branded 404
   page with a way home is served with a 404 status.

---

### User Story 5 - Ship the site as one small container (Priority: P1)

The maintainer builds the site into a single small container image that the production
composition can run, with nothing in the repository describing where it is hosted.

**Why this priority**: without it nothing the other stories build can go live.

**Independent Test**: build the production image, run it read-only with the production
composition's settings, and confirm every page and the health check answer.

**Acceptance Scenarios**:

1. **Given** the repository at a release tag, **When** the production image is built, **Then** it
   holds only the built site and a web server, starts unprivileged on port 8080, and answers
   `/healthz` with 200.
2. **Given** `compose.production.yaml`, **When** it is validated against the production
   composition contract, **Then** it declares one published service with its image, an HTTP
   readiness probe, no backup, a memory limit, a read-only root filesystem with `/tmp` as tmpfs,
   and passes the hosting policy.
3. **Given** `.env.example`, **When** it is read, **Then** it lists every setting the production
   composition reads, each with a safe example value, and every other `.env*` file is ignored.
4. **Given** the image is running, **When** a static asset and a page are requested, **Then** the
   asset is compressed and cached long-term, the page is not cached, and security headers are
   present on both.
5. **Given** the finished migration, **When** the repository is searched, **Then** it contains no
   deploy script, no host proxy configuration, no registry credential file, no CD workflow, and no
   host name, address, server path or prose about hosting.

---

### User Story 6 - Publish a new episode (Priority: P2)

When a new episode is released on the podcast host, the maintainer brings it onto the site
without editing any page.

**Why this priority**: no episodes have been released for years, but the path must exist.

**Independent Test**: add an item to the feed snapshot, rebuild, and confirm it appears first in
the archive with its own page.

**Acceptance Scenarios**:

1. **Given** a new episode in the public feed, **When** the maintainer runs the feed refresh and
   rebuilds, **Then** the episode appears first in the archive with its own page, and the committed
   snapshot is updated to match.
2. **Given** the feed cannot be reached at build time, **When** the site is built, **Then** the
   build uses the committed snapshot, says so, and still succeeds.
3. **Given** the feed returns something unusable, **When** the site is built, **Then** the build
   uses the snapshot rather than publishing an empty or broken archive.

---

### User Story 7 - Retire the old site safely (Priority: P2)

Each part of the old site is removed only once its replacement is confirmed working by the user.

**Why this priority**: this is a full rewrite and cleanup; deleting early loses the reference.

**Independent Test**: for each replaced area, the user signs off its manual test group, then the
matching old files are deleted in a commit listing them.

**Acceptance Scenarios**:

1. **Given** a replaced area that the user has not yet signed off, **When** cleanup runs, **Then**
   none of its old files are deleted.
2. **Given** the user has signed off an area, **When** cleanup runs, **Then** its old files are
   deleted and the pull request lists each deletion grouped by what replaced it.
3. **Given** the migration is complete, **When** the repository is inspected, **Then** nothing in
   it belongs to the old site, and the README describes only the new one.

### Edge Cases

- An episode without a number in its address (for example `/show/the-mvp`) keeps that address.
- An episode with no image, no keywords or no duration still renders, with that element omitted.
- Artwork that is still loading, or fails to load, from the podcast host shows the default
  placeholder in its space, never a blank box or a broken-image icon.
- Show notes containing HTML are rendered safely: scripts and inline event handlers are removed.
- A keyword that maps to one episode still gets its own filtered list.
- A keyword list whose last page holds fewer than 12 episodes still fills its grid from the start
  without gaps mid-row.
- A Listen / Subscribe platform that has shut down is not offered.
- A visitor double-clicks Send: one message is sent.
- A very long message is limited before sending, and the visitor is told the limit.
- The page is viewed at 320 px wide, at 200% zoom, and in a high-contrast mode.
- Requests for old API addresses (`/api/...`) return 404, not an error page from a missing backend.
- A build with no analytics domain (local development, tests) emits no analytics script, and the
  test suite sees no failed analytics requests.

## Clarifications

### Session 2026-10-11

- Q: Keep the weekly next-show countdown and the live Twitch player on the home page? → A: Drop
  both.
- Q: Analytics? → A: Cookieless Plausible, disclosed in the privacy policy.
- Q: Should the redesigned site use a dark theme, a light theme, or follow each visitor's system
  setting? → A: Dark by default, switching to light when the visitor's system prefers light; no
  manual toggle.
- Q: How many episodes should each page of the podcast archive show? → A: 12 per page, laid out
  as a card grid of 1 to 4 columns depending on screen width.
- Q: Should the Discord widget on the contact page load straight away, or only when the visitor
  clicks to show it? → A: Neither; remove the widget and offer a plain link to the Discord invite.
- Q: Should episode artwork be downloaded at build time and served from the site, or loaded
  straight from the podcast host? → A: Loaded from the podcast host, lazily and asynchronously;
  loading from the host takes priority over load speed, and a default placeholder image shows
  until each artwork has loaded.
- Q: What should the home page show of the podcast? → A: The latest episode only: its title, its
  artwork (lazy-loaded from the podcast host, placeholder until loaded) and a link to its episode
  page; no date.

## Requirements *(mandatory)*

### Functional Requirements

**Content and navigation**

- **FR-001**: The site MUST provide these pages at their current addresses: home `/`, archive
  `/show-archive`, keyword archive `/show-archive/<keyword>`, episode `/show/<slug>`, `/contact`,
  `/faq`, `/privacy`, `/ethics`, `/community-guidelines`, and a 404 page.
- **FR-002**: Every page MUST share one header (brand, Home, Podcast Archive, Listen / Subscribe,
  Contact) and one footer (site links, policy links, social links, copyright).
- **FR-003**: The Listen / Subscribe menu MUST list only platforms that still operate, each linking
  to the show on that platform.
- **FR-004**: Social links MUST point directly at each DNI profile (GitHub, X/Twitter, Twitch,
  YouTube, Discord, LinkedIn).
- **FR-005**: The home page MUST show the introduction, the three feature cards and the creators,
  from content data rather than markup.
- **FR-005a**: The home page MUST show the latest episode, taken from the same episode data as the
  archive: its title, its artwork (loaded as FR-011a requires) and a link to its episode page. It
  MUST NOT show the episode's date.
- **FR-006**: FAQ, privacy and cookie policy, ethics and community guidelines content MUST be
  carried over, with the privacy and cookie policy rewritten to match the new site (FR-024).

**Podcast archive**

- **FR-007**: The archive MUST list every episode newest first, 12 per page, as a card grid of 1
  to 4 columns that grows with the screen width, with links to the previous, next, first and last
  pages and each page having its own address. Keyword lists page the same way.
- **FR-008**: Each list entry MUST show title, image, summary, keywords, publish date and duration.
- **FR-009**: The archive MUST show the most-used keywords with their episode counts, and each
  keyword MUST have its own paged list.
- **FR-010**: Each episode MUST have a page at `/show/<slug>`, where the slug is the last segment
  of the episode's link on the podcast host, lowercased, matching the current site's addresses.
- **FR-011**: An episode page MUST show title, image, publish date, duration, keywords, an in-page
  audio player, a download link, a link to the episode on the podcast host, and the full show
  notes, with unsafe markup removed.
- **FR-011a**: Episode artwork MUST be loaded from the podcast host, not copied into the site.
  Every artwork image MUST load lazily and decode asynchronously, and MUST reserve its space
  before it loads so the layout does not shift, as the constitution requires for third-party
  images. A default placeholder image, served by the site, MUST fill that space from first paint
  until the artwork has loaded, and MUST remain if the artwork fails to load. Loading from the
  podcast host takes priority over load speed.
- **FR-012**: Episode data MUST come from the show's public feed at build time; when the feed is
  unreachable or unusable, the build MUST use a committed snapshot and report that it did.
- **FR-013**: The maintainer MUST be able to refresh the committed snapshot from the feed with one
  command, and a new episode MUST appear after a rebuild without editing any template.
- **FR-014**: Archive browsing, paging and keyword filtering MUST work without JavaScript.

**Contact**

- **FR-015**: The contact form MUST collect name and email (both required) and a message (required,
  at most 5,000 characters), and MUST send it to the team's mailbox through a third-party form
  service, with no server of the site's own.
- **FR-016**: The form MUST require a managed human check and MUST include a hidden honeypot
  field; submissions failing either MUST NOT reach the team.
- **FR-017**: With JavaScript the form MUST submit in place and announce sending, success and
  failure in an accessible live region, disable Send while sending, and keep the visitor's input
  on failure; without JavaScript it MUST post natively.
- **FR-018**: The contact page MUST also offer a plain link to the Discord invite and the social
  profiles. It MUST NOT embed a Discord widget or any other third-party frame.

**Design, speed and access**

- **FR-019**: The site MUST have a new, modern, responsive design that keeps the DNI brand (logo,
  orange and dark purple), working from 320 px wide upward. It MUST be dark by default and
  switch to a light variant when the visitor's system prefers light, with no manual toggle; both
  variants MUST meet the contrast requirement of FR-023.
- **FR-020**: Every page MUST have a unique title and description, social sharing metadata and a
  canonical address, and appear in a generated sitemap; episode pages MUST carry podcast episode
  structured data.
- **FR-021**: The site MUST keep advertising the podcast feed for discovery on every page.
- **FR-022**: Each page's own styling and scripting MUST stay within the budget the constitution
  sets, and a test MUST fail when a page exceeds it.
- **FR-023**: Every page MUST meet WCAG 2.2 AA.
- **FR-024**: The privacy and cookie policy MUST name each third party the site contacts and MUST
  NOT name dropped services (the old analytics, the old captcha, the site's own mail server). The
  site MUST set no non-essential cookies or storage, so no cookie banner is shown.
- **FR-025**: The site MUST use cookieless Plausible analytics: every page embeds the Plausible
  script served first-party from `/stats/js/s.js` and reporting to `/stats/api/event`, with the
  analytics domain supplied at build time. When no analytics domain is supplied, no analytics
  script is emitted. The privacy policy MUST disclose it.

**Packaging and onboarding**

- **FR-026**: A single production image MUST serve the built site from an unprivileged web server
  on port 8080, answer `/healthz` with 200, run with a read-only root filesystem and `/tmp` as
  tmpfs, and need no added capabilities.
- **FR-027**: The image MUST serve static assets compressed with long-lived cache headers, pages
  with `no-cache`, security headers on every response, and the 404 page for unknown addresses.
- **FR-028**: `compose.production.yaml` MUST define that one service and an `x-webapp` block
  declaring the app id, the image built from the repository with the analytics domain as its
  build argument, the published service, an HTTP probe on `/healthz`, no backup, its memory
  limit, and analytics enabled, and MUST pass the production composition contract and hosting
  policy.
- **FR-029**: `.env.example` MUST be the only committed environment file and MUST list every key
  the production composition reads, including the analytics domain, each with a safe example
  value; `.gitignore` MUST ignore `.env.*` except `.env.example`.
- **FR-030**: The site's version MUST be kept in one place, and each release MUST be a git tag of
  that version.
- **FR-031**: Continuous integration MUST build the site and the production image and run the
  test suite, and MUST NOT publish images or deploy.
- **FR-032**: No committed file, including comments, MUST contain deploy scripts, host proxy
  configuration, credentials, host names, addresses, server paths, or prose about where or how the
  site is hosted.

**Migration and cleanup**

- **FR-033**: Old files MUST be deleted only after the user signs off the manual test group of the
  area that replaces them, and each deletion MUST be listed in its pull request.
- **FR-034**: When the migration is complete, the repository MUST hold nothing from the old site
  (application code, its tests, solution and package files, its helper scripts, its CD workflow,
  its deploy script, host proxy configuration and maintenance page, and registry credential
  files), and the README and worktree preparation settings MUST describe only the new site.

### Key Entities

- **Episode**: title, slug, publish date, duration, summary, show notes (HTML), image, keywords,
  audio file (address, type, size), link on the podcast host. Identified by its slug.
- **Keyword**: a normalised topic label (lowercase, spaces as hyphens) with the episodes carrying
  it and their count.
- **Creator**: name, photo, short biography, links.
- **Listening platform**: name, icon, link to the show on that platform.
- **Feed snapshot**: the committed copy of the public feed the build falls back to, with the date
  it was taken.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Every address listed in FR-001, and all 84 current episode addresses, return their
  page on the new site.
- **SC-002**: On a mid-range phone over a fast 4G connection, every page shows its main content
  within 1.5 seconds, and repeat views within 0.5 seconds, with placeholders standing in for any
  episode artwork not yet loaded. The time artwork takes to arrive from the podcast host is not
  counted.
- **SC-003**: Every page scores 100 for accessibility, best practices and SEO in an automated page
  audit. Pages without episode artwork score at least 95 for performance; pages showing episode
  artwork (home, archive and episode pages) are held to the same score only for the site's own
  resources.
- **SC-004**: A visitor can reach any episode's audio from the home page in three selections or
  fewer.
- **SC-005**: A contact message sent from the site reaches the team's mailbox within 5 minutes,
  and none of 20 automated submissions that fill the honeypot do.
- **SC-006**: The production image is under 30 MB and the running site uses under 48 MB of memory.
- **SC-007**: Publishing a new episode needs one command and a rebuild, with no template edited.
- **SC-008**: At completion, a search of the repository finds no file from the old site and no
  host, credential or deployment detail.

## Assumptions

- The feed at its current public address stays available; its 84 episodes are the complete
  archive, and the newest is from 2022.
- The live site's episode list does not currently work, so the feed, not the live site, is the
  reference for which episodes and episode addresses must exist (SC-001).
- Sorting by title or length is dropped: newest first is the only order, which keeps every list a
  static page. Keyword filtering stays.
- Platforms that have shut down (Google Podcasts, Stitcher) are removed from Listen / Subscribe;
  the rest keep their current links unless a link no longer resolves.
- The contact form drops the "delete my details" and "marketing opt-out" checkboxes: there is no
  mailing list, and the form service only relays the message by email.
- The form service's access key for the team's mailbox is `df519f5e-f071-4f08-adce-93be0833cc8c`.
  It is public by design (it appears in every page's form), and the domains allowed to use it are
  restricted in the form service's own settings.
- Audio and episode artwork are served by the podcast host; the site copies neither.
- The production composition's app id is `dni`.
- Redirects for alternative host names and the old `discord.` and `youtube.` subdomains are out of
  scope; FR-004 removes the site's own dependency on them.
- The current site keeps running until the new one is live, so old files are the reference until
  each area is signed off.
