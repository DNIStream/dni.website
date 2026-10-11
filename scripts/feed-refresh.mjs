// Replaces the committed feed snapshot with the live podcast feed, but only when the live copy
// parses and holds at least as many episodes as the snapshot. Run with `npm run feed:refresh`.
import { readFile, writeFile } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import { FEED_URL, parseFeed } from "../lib/feed.js";

const SNAPSHOT_PATH = fileURLToPath(new URL("../site/_data/feed-snapshot.xml", import.meta.url));

export async function refreshSnapshot({
  snapshotPath = SNAPSHOT_PATH,
  fetchImpl = globalThis.fetch,
  url = FEED_URL,
  timeoutMs = 15000,
} = {}) {
  const before = parseFeed(await readFile(snapshotPath, "utf8")).length;
  const response = await fetchImpl(url, { signal: AbortSignal.timeout(timeoutMs) });
  if (!response.ok) throw new Error(`the feed answered HTTP ${response.status}`);
  const xml = await response.text();
  const after = parseFeed(xml).length;
  if (after < before) throw new Error(`the feed has ${after} episodes, fewer than the ${before} in the snapshot`);
  await writeFile(snapshotPath, xml);
  return { before, after };
}

if (process.argv[1] && fileURLToPath(import.meta.url) === process.argv[1]) {
  try {
    const { before, after } = await refreshSnapshot();
    console.log(`[feed] snapshot updated: ${before} -> ${after} episodes`);
  } catch (error) {
    console.error(`[feed] snapshot left unchanged: ${error.message}`);
    process.exitCode = 1;
  }
}
