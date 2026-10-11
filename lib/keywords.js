export function normaliseKeywords(raw) {
  if (!raw) return [];
  const seen = new Set();
  for (const part of String(raw).split(",")) {
    const keyword = part.trim().toLowerCase().replace(/\s+/g, "-");
    if (keyword) seen.add(keyword);
  }
  return [...seen];
}

export function keywordSlug(keyword) {
  const slug = keyword
    .toLowerCase()
    .replace(/#/g, "-sharp")
    .replace(/\+/g, "-plus")
    .replace(/[^a-z0-9-]/g, "-")
    .replace(/-+/g, "-")
    .replace(/^-|-$/g, "");
  if (slug === "page") {
    throw new Error(`keyword "${keyword}" would collide with the archive's /page/ paths`);
  }
  return slug;
}
