// Layer: build output — which pages exist, and at which addresses, is a fact of the built files.
import { test } from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { page, pages } from "./helpers.mjs";
import { parseFeed } from "../../lib/feed.js";
import { keywordSlug } from "../../lib/keywords.js";

const episodes = parseFeed(readFileSync("site/_data/feed-snapshot.xml", "utf8"));

test("the archive has seven pages at their addresses", () => {
  assert.ok(page("/show-archive/"), "/show-archive/");
  for (let n = 2; n <= 7; n++) assert.ok(page(`/show-archive/page/${n}/`), `/show-archive/page/${n}/`);
  assert.equal(page("/show-archive/page/8/"), undefined);
  assert.equal(page("/show-archive/page/1/"), undefined);
});

test("every episode has its own page at /show/<slug>/", () => {
  assert.equal(episodes.length, 84);
  for (const { slug } of episodes) assert.ok(page(`/show/${slug}/`), `/show/${slug}/`);
  assert.equal(pages.filter((p) => p.url.startsWith("/show/")).length, 84);
});

test("every keyword has a list page, with extra pages past twelve episodes", () => {
  const counts = new Map();
  for (const e of episodes) for (const k of e.keywords) counts.set(k, (counts.get(k) ?? 0) + 1);
  for (const [keyword, count] of counts) {
    const root = `/show-archive/${keywordSlug(keyword)}/`;
    assert.ok(page(root), root);
    const total = Math.ceil(count / 12);
    for (let n = 2; n <= total; n++) assert.ok(page(`${root}page/${n}/`), `${root}page/${n}/`);
    assert.equal(page(`${root}page/${total + 1}/`), undefined);
  }
});

test("symbol keywords get safe addresses", () => {
  assert.ok(page("/show-archive/c-sharp/"));
  assert.ok(page("/show-archive/c-plus-plus/"));
});
