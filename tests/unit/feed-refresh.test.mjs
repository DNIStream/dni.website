import { test, beforeEach, afterEach } from "node:test";
import assert from "node:assert/strict";
import { mkdtempSync, readFileSync, writeFileSync, rmSync } from "node:fs";
import { tmpdir } from "node:os";
import { join } from "node:path";
import { refreshSnapshot } from "../../scripts/feed-refresh.mjs";
import { parseFeed } from "../../lib/feed.js";

const SNAPSHOT = readFileSync("site/_data/feed-snapshot.xml", "utf8");

const NEW_ITEM = `<item>
      <title>Ep 85 - Back Again</title>
      <link>https://dnistream.fireside.fm/85-back-again</link>
      <pubDate>Thu, 01 Oct 2026 19:00:00 +0100</pubDate>
      <enclosure url="https://example.test/85.mp3" length="1" type="audio/mpeg"/>
      <itunes:duration>59:00</itunes:duration>
    </item>
    <item>`;

const withNewEpisode = SNAPSHOT.replace("<item>", NEW_ITEM);
const withoutFirstEpisode = SNAPSHOT.replace(/<item>[\s\S]*?<\/item>/, "");
const serve = (body, status = 200) => async () => new Response(body, { status });

let dir;
let path;
beforeEach(() => {
  dir = mkdtempSync(join(tmpdir(), "dni-feed-"));
  path = join(dir, "feed-snapshot.xml");
  writeFileSync(path, SNAPSHOT);
});
afterEach(() => rmSync(dir, { recursive: true, force: true }));

test("writes a feed that parses and has a new episode, which then comes first", async () => {
  const result = await refreshSnapshot({ snapshotPath: path, fetchImpl: serve(withNewEpisode) });
  assert.deepEqual(result, { before: 84, after: 85 });
  const [latest] = parseFeed(readFileSync(path, "utf8"));
  assert.equal(latest.slug, "85-back-again");
});

test("writes a feed with the same number of episodes", async () => {
  const result = await refreshSnapshot({ snapshotPath: path, fetchImpl: serve(SNAPSHOT) });
  assert.deepEqual(result, { before: 84, after: 84 });
});

for (const [name, fetchImpl, message] of [
  ["has fewer episodes", serve(withoutFirstEpisode), /83 episodes, fewer than the 84/],
  ["does not parse", serve("<html>maintenance</html>"), /not an RSS feed/],
  ["answers with an error", serve("", 503), /HTTP 503/],
  ["cannot be reached", async () => { throw new Error("getaddrinfo ENOTFOUND"); }, /ENOTFOUND/],
]) {
  test(`leaves the snapshot alone when the feed ${name}`, async () => {
    await assert.rejects(refreshSnapshot({ snapshotPath: path, fetchImpl }), message);
    assert.equal(readFileSync(path, "utf8"), SNAPSHOT);
  });
}
