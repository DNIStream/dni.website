// Layer: build output — metadata, the sitemap and the analytics switch are facts of the built files.
import { test } from "node:test";
import assert from "node:assert/strict";
import { readFileSync, mkdtempSync, rmSync, readdirSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { spawnSync } from "node:child_process";
import { DIST, page, pages } from "./helpers.mjs";

const indexed = pages.filter((p) => p.url !== "/404.html");
const meta = (html, name) => html.match(new RegExp(`<meta (?:name|property)="?${name}"? content="([^"]*)"`))?.[1];
const title = (html) => html.match(/<title>([^<]*)<\/title>/)?.[1];

test("every page has a unique title and description", () => {
  const titles = new Map();
  for (const p of indexed) {
    assert.ok(title(p.html), `${p.url} title`);
    assert.ok(meta(p.html, "description"), `${p.url} description`);
    assert.equal(titles.get(title(p.html)), undefined, `${p.url} repeats the title of ${titles.get(title(p.html))}`);
    titles.set(title(p.html), p.url);
  }
});

test("every page has a canonical link, Open Graph tags and the podcast feed", () => {
  for (const p of pages) {
    assert.match(p.html, /<link rel="?canonical"? href="?http/, `${p.url} canonical`);
    for (const og of ["og:title", "og:description", "og:url", "og:image"]) assert.ok(meta(p.html, og), `${p.url} ${og}`);
    assert.match(p.html, /<link rel="?alternate"? type="?application\/rss\+xml"? [^>]*href="?https:\/\/feeds\.fireside\.fm\/dnistream\/rss/, `${p.url} feed`);
  }
});

test("the sitemap lists every page except the 404 page", () => {
  const sitemap = readFileSync(join(DIST, "sitemap.xml"), "utf8");
  const listed = new Set([...sitemap.matchAll(/<loc>[^<]*?:\/\/[^/]+(\/[^<]*)<\/loc>/g)].map((m) => m[1]));
  for (const p of indexed) assert.ok(listed.has(p.url), `${p.url} missing from the sitemap`);
  assert.equal(listed.size, indexed.length);
  assert.match(page("/404.html").html, /<meta name="?robots"? content="?noindex/);
});

test("robots.txt allows everything and names the sitemap", () => {
  const robots = readFileSync(join(DIST, "robots.txt"), "utf8");
  assert.match(robots, /^User-agent: \*$/m);
  assert.match(robots, /^Allow: \/$/m);
  assert.match(robots, /^Sitemap: http\S+\/sitemap\.xml$/m);
});

test("the web manifest is valid JSON with the site's icons", () => {
  const manifest = JSON.parse(readFileSync(join(DIST, "site.webmanifest"), "utf8"));
  assert.equal(manifest.name, "DNI Stream");
  assert.ok(manifest.icons.length >= 2);
});

test("analytics is emitted only when a domain is set at build time", () => {
  for (const p of pages) assert.doesNotMatch(p.html, /\/stats\/js\//, `${p.url} has analytics without a domain`);

  const out = mkdtempSync(join(tmpdir(), "dni-plausible-"));
  try {
    const run = spawnSync(process.execPath, ["node_modules/@11ty/eleventy/cmd.cjs", `--output=${out}`, "--quiet"], {
      env: { ...process.env, FEED_OFFLINE: "1", PLAUSIBLE_DOMAIN: "example.com" },
      encoding: "utf8",
    });
    assert.equal(run.status, 0, run.stderr);
    const html = readFileSync(join(out, "index.html"), "utf8");
    assert.match(html, /<script defer(?:="defer")? data-domain="?example\.com"? data-api="?\/stats\/api\/event"? src="?\/stats\/js\/s\.js"?>/);
    const episodes = readdirSync(join(out, "show"));
    assert.match(readFileSync(join(out, "show", episodes[0], "index.html"), "utf8"), /data-api="?\/stats\/api\/event/);
  } finally {
    rmSync(out, { recursive: true, force: true });
  }
});
