# DNI Website Constitution

## Core Principles

### I. Static by Default

The site is a set of pre-built static files and nothing else.

- The site MUST have no server-side application code, database or API of its own.
- Data from other services (such as the podcast feed) MUST be fetched at build time and rendered
  into the static output. Visitors' browsers MUST NOT depend on a third-party API to see content.
- The one permitted runtime call to a third party is a form submission made by the visitor's own
  browser, as defined under Technology and Content Constraints.
- Every page MUST render its content without JavaScript. JavaScript only enhances.

Rationale: static output is fast, cheap to host, has almost no attack surface and cannot go down
for want of a backend.

### II. Lightweight and Fast

Every byte served needs a reason.

- No front-end framework, no CSS framework, no jQuery, and no icon font. Behaviour is small,
  dependency-free vanilla JavaScript; icons are inline SVG.
- Each page's first-party CSS plus JavaScript MUST stay within 30 KB minified (fonts and images
  excluded), and a test MUST fail when it does not.
- First-party images MUST be sized for their slot, served in a modern format with a fallback
  where needed, and lazy-loaded below the fold. Fonts MUST be self-hosted WOFF2 with
  `font-display: swap`.
- Images owned and served by a third party (such as episode artwork on the podcast host) are
  exempt from sizing and format, but MUST load lazily, decode asynchronously, and reserve their
  space before loading so the layout does not shift.
- HTML, CSS and JavaScript are minified at build. Static assets are served compressed and with
  long-lived cache headers; HTML is served with `no-cache`.

Rationale: "lightning fast" is a requirement, and only a budget that a test enforces keeps it.

### III. Template-Driven Content

Common chrome is written once, and content is data.

- The `<head>`, header, navigation and footer MUST each exist in exactly one layout or partial.
  No page duplicates them.
- Pages carry front matter and their unique body only. Lists (such as episodes) MUST be generated
  from a data source or a collection, never hand-maintained in markup.
- Adding a podcast episode MUST NOT require editing any template. A rebuild picks it up.
- Public URLs MUST stay stable. A URL that moves MUST keep working through a redirect served by
  the site itself.

Rationale: one source per element means a change lands everywhere, and nothing drifts.

### IV. Responsive, Accessible, Modern

The site works for everyone, on every screen, and looks current.

- Layout is mobile-first and MUST work from 320 px wide upward with no horizontal scrolling.
- Pages MUST meet WCAG 2.2 AA: semantic landmarks, labelled controls, visible focus, sufficient
  contrast, and motion that honours `prefers-reduced-motion`.
- The visual design MUST use a single set of design tokens (colour, type scale, spacing) defined
  once as CSS custom properties.
- Every page MUST have a unique title and description, Open Graph metadata, and appear in a
  generated sitemap.

Rationale: a site that is fast but unusable on a phone, or for a screen-reader user, has failed.

### V. One Lightweight Container

The site ships as one small, locked-down container image.

- A multi-stage build produces the static output and copies it into a single unprivileged nginx
  Alpine image listening on port 8080. Nothing else runs in it.
- The image MUST answer `/healthz` with 200, run with a read-only root filesystem (tmpfs for
  `/tmp` only), and need no added capabilities.
- `compose.production.yaml` at the repository root is the only production composition. Its
  `x-webapp` block MUST stay valid, and any change to it bumps the site's version.
- `.env.example` is the only committed environment file. Every other `.env*` file MUST be
  gitignored. No value in any committed file may be a secret.
- The repository MUST NOT contain deploy scripts, deploy steps in CI, host names, addresses,
  domains used for hosting, server paths, users, credentials, or any prose or code comment about
  where or how the site is hosted. CI MAY build and test; it MUST NOT publish.

Rationale: the smaller the image and the less the repository knows about its host, the less there
is to break, leak or maintain.

### VI. Verified by Test

Behaviour is proven, not asserted.

- A behaviour change starts with a failing test (Red-Green-Refactor).
- A Playwright suite runs against the built output served by the production image and asserts,
  per page: a 2xx response, the expected title and first heading, navigation and footer present,
  every first-party image decodes, and no first-party console errors or failed requests.
- Interactive behaviour (navigation toggle, contact form states, paging) has its own test.
- Automated accessibility checks run on every page template, and the payload budget from
  Principle II is a test.

Rationale: a rewrite of every page is only safe if every page is checked, every time.

## Technology and Content Constraints

- **Build**: Eleventy with Nunjucks layouts and Markdown content, on the current Node LTS. Build
  tooling stays in `devDependencies`; the image contains only the built files and nginx.
- **Contact form**: a plain HTML form that posts directly to Web3Forms, using its managed hCaptcha
  and a honeypot field. It MUST work without JavaScript (native post) and, with JavaScript, submit
  in place with an accessible live status for sending, success and failure. The Web3Forms access
  key is a public key and lives in the site's data file. There is no backend of our own.
- **Podcast list**: episodes are rendered as a static, paged list with one page per episode. Data
  comes from the show's public Fireside feed at build time where that is possible; otherwise from
  a committed data file in the same shape, so the switch changes no template.
- **Privacy**: every third party the site contacts (form service, captcha, any font or media
  host) MUST be disclosed in the privacy and cookie policy pages. The site sets no non-essential
  cookies or storage; if that ever changes, a consent banner is required first.
- **No tracking** beyond cookieless, disclosed analytics, and only when its configuration is
  supplied at build time.

## Migration and Cleanup Workflow

The current Angular and .NET site is replaced in full, not adapted.

- New work is built alongside the old code until it replaces it.
- An obsolete file or directory is deleted only after the part that replaces it is working and
  the user has confirmed it by hand (its manual test group signed off). Deleting before
  confirmation is a violation.
- Every deletion is listed in the pull request that makes it, grouped by what replaced it.
- Deployment-related material that the new model does not need (deploy scripts, host proxy
  configs, registry credentials, CD workflows) is removed as part of the migration, under the
  same confirmation rule.
- The repository is done when nothing in it belongs to the old site.

## Governance

This constitution supersedes every other practice in this repository. Plans, specs and reviews
MUST check compliance with it, and any deviation MUST be justified in the plan's complexity
tracking.

- **Amendment**: by pull request that edits this file, states the reason, and updates any spec or
  plan the change affects.
- **Versioning**: semantic. MAJOR removes or redefines a principle; MINOR adds a principle or
  section or materially expands one; PATCH clarifies wording.
- **Review**: each pull request is checked against Principles I–VI and the Migration and Cleanup
  Workflow before merge.

**Version**: 1.1.0 | **Ratified**: 2026-10-10 | **Last Amended**: 2026-10-11
