import { loadEpisodes } from "../../lib/feed.js";
import { keywordSlug } from "../../lib/keywords.js";
import { paginate } from "../../lib/paging.js";

const ARCHIVE_ROOT = "/show-archive/";
const TOP_KEYWORDS = 10;

export default async function () {
  const { source, episodes } = await loadEpisodes({ offline: process.env.FEED_OFFLINE === "1" });

  const byKeyword = new Map();
  for (const episode of episodes) {
    for (const name of episode.keywords) {
      if (!byKeyword.has(name)) byKeyword.set(name, []);
      byKeyword.get(name).push(episode);
    }
  }

  const slugs = new Map();
  const keywords = [...byKeyword].map(([name, list]) => {
    const slug = keywordSlug(name);
    if (slugs.has(slug)) throw new Error(`keywords "${slugs.get(slug)}" and "${name}" share the slug "${slug}"`);
    slugs.set(slug, name);
    return { name, slug, episodes: list, count: list.length, href: `${ARCHIVE_ROOT}${slug}/` };
  });
  const keywordByName = Object.fromEntries(keywords.map((k) => [k.name, k]));

  const topKeywords = [...keywords]
    .sort((a, b) => b.count - a.count || a.name.localeCompare(b.name))
    .slice(0, TOP_KEYWORDS);

  const keywordPages = keywords.flatMap((keyword) =>
    paginate(keyword.episodes, keyword.href).map((page) => ({ ...page, keyword })),
  );

  console.log(`[episodes] ${episodes.length} episodes, ${keywords.length} keywords, from the ${source}`);

  return {
    source,
    episodes,
    latest: episodes[0] ?? null,
    keywords,
    keywordByName,
    topKeywords,
    archivePages: paginate(episodes, ARCHIVE_ROOT),
    keywordPages,
  };
}
