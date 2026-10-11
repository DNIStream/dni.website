import { readFile } from "node:fs/promises";
import { XMLParser } from "fast-xml-parser";
import sanitizeHtml from "sanitize-html";
import { normaliseKeywords } from "./keywords.js";
import { sanitizeNotes } from "./sanitize.js";

export const FEED_URL = "https://feeds.fireside.fm/dnistream/rss";
const SNAPSHOT_URL = new URL("../site/_data/feed-snapshot.xml", import.meta.url);
const SLUG = /^[a-z0-9-]+$/;
const SUMMARY_LENGTH = 200;

const parser = new XMLParser({
  ignoreAttributes: false,
  attributeNamePrefix: "@",
  parseTagValue: false,
  trimValues: true,
  isArray: (name) => name === "item",
});

const text = (value) => {
  if (value === undefined || value === null) return "";
  if (typeof value === "object") return String(value["#text"] ?? "").trim();
  return String(value).trim();
};

const plain = (html) =>
  sanitizeHtml(html, { allowedTags: [], allowedAttributes: {} }).replace(/\s+/g, " ").trim();

function parseDuration(raw) {
  const value = text(raw);
  if (!value) return null;
  const parts = value.split(":").map(Number);
  if (parts.some((n) => !Number.isFinite(n))) return null;
  return parts.reduce((total, n) => total * 60 + n, 0);
}

function durationLabel(seconds) {
  if (seconds === null) return "";
  const hours = Math.floor(seconds / 3600);
  const minutes = Math.floor((seconds % 3600) / 60);
  return hours ? `${hours}h ${String(minutes).padStart(2, "0")}m` : `${minutes}m`;
}

function toEpisode(item, channelImage, index) {
  const title = text(item.title);
  const hostUrl = text(item.link);
  const published = new Date(text(item.pubDate));
  const enclosure = item.enclosure;
  if (!title) throw new Error(`feed item ${index} has no title`);
  if (!text(item.pubDate) || Number.isNaN(published.getTime())) {
    throw new Error(`feed item "${title}" has no valid pubDate`);
  }
  if (!enclosure?.["@url"]) throw new Error(`feed item "${title}" has no enclosure`);

  const slug = hostUrl.split("/").filter(Boolean).pop()?.toLowerCase() ?? "";
  if (!SLUG.test(slug)) throw new Error(`feed item "${title}" has an unsafe slug "${slug}"`);

  const description = text(item.description);
  const notesSource = text(item["content:encoded"]) || description;
  const subtitle = text(item["itunes:subtitle"]);
  const durationSeconds = parseDuration(item["itunes:duration"]);

  return {
    slug,
    title,
    summary: subtitle || plain(description).slice(0, SUMMARY_LENGTH).trim(),
    notesHtml: sanitizeNotes(notesSource),
    published,
    durationSeconds,
    durationLabel: durationLabel(durationSeconds),
    image: item["itunes:image"]?.["@href"] || channelImage || null,
    keywords: normaliseKeywords(text(item["itunes:keywords"])),
    audio: {
      url: enclosure["@url"],
      type: enclosure["@type"] || "audio/mpeg",
      length: enclosure["@length"] ? Number(enclosure["@length"]) : null,
    },
    hostUrl,
  };
}

export function parseFeed(xml) {
  const doc = parser.parse(xml);
  const channel = doc?.rss?.channel;
  if (!channel) throw new Error("not an RSS feed");
  const channelImage = channel["itunes:image"]?.["@href"] || null;
  const episodes = (channel.item || []).map((item, i) => toEpisode(item, channelImage, i));

  const seen = new Set();
  for (const { slug } of episodes) {
    if (seen.has(slug)) throw new Error(`duplicate episode slug "${slug}"`);
    seen.add(slug);
  }
  // Array.prototype.sort is stable, so episodes published together keep feed order.
  return episodes.sort((a, b) => b.published - a.published);
}

async function fetchFeed(fetchImpl, url, timeoutMs) {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), timeoutMs);
  try {
    const response = await fetchImpl(url, { signal: controller.signal });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return await response.text();
  } finally {
    clearTimeout(timer);
  }
}

export async function readSnapshot() {
  return readFile(SNAPSHOT_URL, "utf8");
}

export async function loadEpisodes({
  offline = false,
  fetchImpl = globalThis.fetch,
  timeoutMs = 15000,
  url = FEED_URL,
  log = console.log,
} = {}) {
  if (!offline) {
    try {
      const episodes = parseFeed(await fetchFeed(fetchImpl, url, timeoutMs));
      if (episodes.length === 0) throw new Error("the feed has no episodes");
      return { source: "live", episodes };
    } catch (error) {
      log(`[feed] live feed unusable (${error.message}); using the committed snapshot`);
    }
  }
  return { source: "snapshot", episodes: parseFeed(await readSnapshot()) };
}
