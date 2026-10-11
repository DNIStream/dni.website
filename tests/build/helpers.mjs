import { readdirSync, readFileSync, statSync, existsSync } from "node:fs";
import { join, relative, sep } from "node:path";

export const DIST = "dist";

if (!existsSync(DIST)) {
  throw new Error("dist/ is missing: run `FEED_OFFLINE=1 npm run build` before `npm run test:build`");
}

function walk(dir) {
  return readdirSync(dir, { withFileTypes: true }).flatMap((entry) => {
    const path = join(dir, entry.name);
    return entry.isDirectory() ? walk(path) : [path];
  });
}

/** Every built HTML page: its public URL, file path and markup. */
export const pages = walk(DIST)
  .filter((path) => path.endsWith(".html"))
  .map((file) => {
    const rel = relative(DIST, file).split(sep).join("/");
    const url = "/" + rel.replace(/(^|\/)index\.html$/, "$1").replace(/^\/$/, "");
    return { url: url === "/" ? "/" : url, file, html: readFileSync(file, "utf8") };
  });

export const page = (url) => pages.find((p) => p.url === url);

export const fileSize = (publicPath) => statSync(join(DIST, publicPath.split("?")[0])).size;

/** Values of one attribute on every element of a tag, e.g. attrs(html, "script", "src"). */
export function attrs(html, tag, attr) {
  const re = new RegExp(`<${tag}\\b[^>]*?\\s${attr}=("([^"]*)"|'([^']*)'|([^\\s>]+))`, "gi");
  return [...html.matchAll(re)].map((m) => m[2] ?? m[3] ?? m[4]);
}

export function stylesheets(html) {
  return [...html.matchAll(/<link\b[^>]*>/gi)]
    .map((m) => m[0])
    .filter((tag) => /rel=("|')?stylesheet/i.test(tag))
    .map((tag) => tag.match(/href=("([^"]*)"|'([^']*)'|([^\s>]+))/i))
    .filter(Boolean)
    .map((m) => m[2] ?? m[3] ?? m[4]);
}

export const isLocal = (url) => url.startsWith("/") && !url.startsWith("//");
