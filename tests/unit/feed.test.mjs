import { test } from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { parseFeed, loadEpisodes } from "../../lib/feed.js";

const SNAPSHOT = new URL("../../site/_data/feed-snapshot.xml", import.meta.url);

const item = (fields) => `<item>${fields}</item>`;
const channel = (items, image = "https://img.example/show.jpg") => `<?xml version="1.0"?>
<rss xmlns:itunes="http://www.itunes.com/dtds/podcast-1.0.dtd" xmlns:content="http://purl.org/rss/1.0/modules/content/">
<channel><title>Show</title><itunes:image href="${image}"/>${items.join("")}</channel></rss>`;

const full = (slug, date, extra = "") =>
  item(`<title> ${slug} title </title><link>https://host.example/${slug}</link>
<pubDate>${date}</pubDate><enclosure url="https://cdn.example/${slug}.mp3" type="audio/mpeg" length="123"/>${extra}`);

test("parses every episode in the committed snapshot, newest first", () => {
  const episodes = parseFeed(readFileSync(SNAPSHOT, "utf8"));
  assert.equal(episodes.length, 84);
  for (let i = 1; i < episodes.length; i++) {
    assert.ok(episodes[i - 1].published >= episodes[i].published);
  }
  const slugs = episodes.map((e) => e.slug);
  assert.ok(slugs.every((s) => /^[a-z0-9-]+$/.test(s)));
  assert.equal(new Set(slugs).size, slugs.length);
  assert.ok(slugs.includes("84-debugging"));
  assert.ok(slugs.includes("the-mvp"));
});

test("maps fields per the data model", () => {
  const xml = channel([
    full(
      "7-Thing",
      "Tue, 02 Aug 2022 18:00:00 +0000",
      `<itunes:subtitle>Short summary</itunes:subtitle><itunes:duration>1:04:27</itunes:duration>
<itunes:keywords>Dev, Game Development</itunes:keywords><itunes:image href="https://img.example/ep.jpg"/>
<content:encoded><![CDATA[<p>Notes</p><iframe src="https://p.example"></iframe>]]></content:encoded>`,
    ),
  ]);
  const [e] = parseFeed(xml);
  assert.equal(e.slug, "7-thing");
  assert.equal(e.title, "7-Thing title");
  assert.equal(e.summary, "Short summary");
  assert.equal(e.notesHtml, "<p>Notes</p>");
  assert.equal(e.durationSeconds, 3867);
  assert.equal(e.durationLabel, "1h 04m");
  assert.equal(e.image, "https://img.example/ep.jpg");
  assert.deepEqual(e.keywords, ["dev", "game-development"]);
  assert.deepEqual(e.audio, { url: "https://cdn.example/7-Thing.mp3", type: "audio/mpeg", length: 123 });
  assert.equal(e.hostUrl, "https://host.example/7-Thing");
  assert.equal(e.published.toISOString(), "2022-08-02T18:00:00.000Z");
});

test("falls back for optional fields", () => {
  const long = "word ".repeat(80);
  const xml = channel([
    item(`<title>T</title><link>https://host.example/a</link><pubDate>Mon, 01 Aug 2022 00:00:00 +0000</pubDate>
<enclosure url="https://cdn.example/a.mp3"/><description><![CDATA[<p>${long}</p>]]></description>`),
  ]);
  const [e] = parseFeed(xml);
  assert.equal(e.image, "https://img.example/show.jpg");
  assert.equal(e.audio.type, "audio/mpeg");
  assert.equal(e.durationSeconds, null);
  assert.equal(e.durationLabel, "");
  assert.deepEqual(e.keywords, []);
  assert.ok(e.summary.length <= 200);
  assert.doesNotMatch(e.summary, /</);
  assert.match(e.notesHtml, /^<p>word/);
});

test("parses MM:SS and plain-second durations", () => {
  const xml = channel([
    full("a", "Mon, 01 Aug 2022 00:00:00 +0000", "<itunes:duration>47:05</itunes:duration>"),
    full("b", "Sun, 31 Jul 2022 00:00:00 +0000", "<itunes:duration>125</itunes:duration>"),
  ]);
  const [a, b] = parseFeed(xml);
  assert.equal(a.durationSeconds, 2825);
  assert.equal(a.durationLabel, "47m");
  assert.equal(b.durationSeconds, 125);
  assert.equal(b.durationLabel, "2m");
});

test("keeps feed order for episodes published at the same time", () => {
  const date = "Mon, 01 Aug 2022 00:00:00 +0000";
  const slugs = parseFeed(channel([full("x", date), full("y", date)])).map((e) => e.slug);
  assert.deepEqual(slugs, ["x", "y"]);
});

test("rejects a duplicate slug", () => {
  const date = "Mon, 01 Aug 2022 00:00:00 +0000";
  assert.throws(() => parseFeed(channel([full("x", date), full("x", date)])), /duplicate/i);
});

test("rejects an item missing a title, date or audio", () => {
  assert.throws(() => parseFeed(channel([item("<link>https://h.example/a</link>")])), /title|pubDate|enclosure/i);
});

test("rejects a slug with unsafe characters", () => {
  assert.throws(
    () => parseFeed(channel([full("bad_slug", "Mon, 01 Aug 2022 00:00:00 +0000")])),
    /slug/i,
  );
});

const snapshotXml = readFileSync(SNAPSHOT, "utf8");
const quiet = { log: () => {} };

test("uses the live feed when it answers", async () => {
  const live = channel([full("live-one", "Mon, 01 Aug 2022 00:00:00 +0000")]);
  const result = await loadEpisodes({ ...quiet, fetchImpl: async () => new Response(live) });
  assert.equal(result.source, "live");
  assert.deepEqual(result.episodes.map((e) => e.slug), ["live-one"]);
});

for (const [name, fetchImpl] of [
  ["the fetch rejects", async () => { throw new Error("offline"); }],
  ["the response is not XML", async () => new Response("<html>oops</html>")],
  ["the feed has no items", async () => new Response(channel([]))],
  ["the response is an error", async () => new Response("nope", { status: 503 })],
]) {
  test(`falls back to the snapshot when ${name}`, async () => {
    const lines = [];
    const result = await loadEpisodes({ fetchImpl, log: (l) => lines.push(l) });
    assert.equal(result.source, "snapshot");
    assert.equal(result.episodes.length, 84);
    assert.equal(lines.length, 1);
    assert.match(lines[0], /snapshot/);
  });
}

test("falls back when the fetch exceeds the timeout", async () => {
  const hang = (_url, { signal }) =>
    new Promise((_, reject) => signal.addEventListener("abort", () => reject(new Error("aborted"))));
  const result = await loadEpisodes({ ...quiet, fetchImpl: hang, timeoutMs: 20 });
  assert.equal(result.source, "snapshot");
});

test("never fetches when offline", async () => {
  let called = false;
  const result = await loadEpisodes({
    ...quiet,
    offline: true,
    fetchImpl: async () => { called = true; return new Response(snapshotXml); },
  });
  assert.equal(called, false);
  assert.equal(result.source, "snapshot");
});
