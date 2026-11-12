# Manual test plan: Node.js 26 build and Alpine 3.24 / nginx 1.30 image

| ID | Title | Status | Signed off at | Date | Reason |
|---|---|---|---|---|---|
| MT-01 | Site served by the Alpine 3.24 / nginx 1.30 image | untested | | | |
| MT-02 | Live feed read by the Node 26 build | untested | | | |

## MT-01: Site served by the Alpine 3.24 / nginx 1.30 image

Covers: nginx 1.28.3 → 1.30.6 (Alpine 3.22 → 3.24)
Paths: `Dockerfile`, `docker/**`, `compose.yaml`

### Steps
1. Setup: on this branch, start the site with `npm run container:up`, then open http://localhost:8080. The home page looks as it does on the live site: header, latest episode, listen links, footer.
2. Open an episode page from the home page, then the show archive, then a keyword link on an episode. Each page renders fully, with styles, icons and the Montserrat font.
3. Open devtools → Network and reload the home page. The document response carries `Content-Security-Policy`, `Strict-Transport-Security`, `X-Content-Type-Options` and `Cache-Control: no-cache`.
4. In the same panel, select the CSS file. It is served with `Content-Encoding: gzip` and a long `max-age`.

### Negative cases
- Open http://localhost:8080/does-not-exist. The site's own 404 page appears, styled, and the Network panel shows status 404 — not nginx's default error page.
- No page shows a browser console error about a blocked script, style or font.

## MT-02: Live feed read by the Node 26 build

Covers: node:24-alpine → node:26-alpine build stage (Undici 8 `fetch`)
Paths: `Dockerfile`, `lib/feed.js`, `package.json`, `package-lock.json`

### Steps
1. With the site from MT-01 running (its image builds against the live feed), open http://localhost:8080/show-archive/. The newest episode listed is the newest episode on the podcast's feed (https://feeds.fireside.fm/dnistream/rss) or on Spotify.
2. Open that newest episode's page. Its title, date, duration, show notes and audio player match the feed.

### Negative cases
- The newest episode in the archive is not older than the newest one on the feed — an older one means the build silently fell back to the committed snapshot because the live fetch failed on Node 26.
