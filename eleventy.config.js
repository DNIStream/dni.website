import { createHash } from "node:crypto";
import { readFileSync, existsSync } from "node:fs";
import { minify } from "html-minifier-terser";

const ICON_DIR = "site/_includes/icons";
const BRAND_DIR = "node_modules/simple-icons/icons";

const iconCache = new Map();
function icon(name, label = "") {
  if (!iconCache.has(name)) {
    const own = `${ICON_DIR}/${name}.svg`;
    const brand = `${BRAND_DIR}/${name}.svg`;
    const file = existsSync(own) ? own : existsSync(brand) ? brand : null;
    if (!file) throw new Error(`unknown icon "${name}"`);
    const svg = readFileSync(file, "utf8")
      .trim()
      .replace(/<title>.*?<\/title>/, "")
      .replace(/\srole="img"/, "");
    iconCache.set(name, svg);
  }
  const a11y = label ? `role="img" aria-label="${label}"` : `aria-hidden="true" focusable="false"`;
  return iconCache.get(name).replace("<svg ", `<svg class="icon" ${a11y} `);
}

const hashCache = new Map();
function assetHash(path) {
  if (!hashCache.has(path)) {
    const file = `site${path}`;
    const digest = createHash("sha256").update(readFileSync(file)).digest("hex").slice(0, 10);
    hashCache.set(path, digest);
  }
  return `${path}?v=${hashCache.get(path)}`;
}

const dateFormat = new Intl.DateTimeFormat("en-GB", { day: "numeric", month: "long", year: "numeric", timeZone: "UTC" });

export default function (eleventyConfig) {
  eleventyConfig.addPassthroughCopy({ "site/assets": "assets" });
  eleventyConfig.addPassthroughCopy({
    "node_modules/@fontsource-variable/montserrat/files/montserrat-latin-wght-normal.woff2":
      "assets/fonts/montserrat-latin-wght-normal.woff2",
  });

  eleventyConfig.addGlobalData("env", {
    siteUrl: (process.env.SITE_URL || "http://localhost:8080").replace(/\/+$/, ""),
    plausibleDomain: process.env.PLAUSIBLE_DOMAIN || "",
  });

  eleventyConfig.addShortcode("icon", icon);
  eleventyConfig.addFilter("asset", assetHash);
  eleventyConfig.addFilter("displayDate", (date) => dateFormat.format(new Date(date)));
  eleventyConfig.addFilter("isoDate", (date) => new Date(date).toISOString());
  eleventyConfig.addFilter("isoDuration", (seconds) => (seconds ? `PT${seconds}S` : ""));
  eleventyConfig.addFilter("json", (value) => JSON.stringify(value).replace(/</g, "\\u003c"));
  eleventyConfig.addFilter("year", () => new Date().getUTCFullYear());

  eleventyConfig.addTransform("minify-html", async function (content) {
    if (!(this.page.outputPath || "").endsWith(".html")) return content;
    return minify(content, {
      collapseWhitespace: true,
      conservativeCollapse: true,
      removeComments: true,
      minifyCSS: true,
      // Inline scripts stay byte-for-byte: the CSP allows the one inline script by its hash.
      minifyJS: false,
    });
  });

  return {
    dir: {
      input: "site",
      includes: "_includes",
      data: "_data",
      output: "dist",
    },
    templateFormats: ["njk", "md"],
    markdownTemplateEngine: "njk",
    htmlTemplateEngine: "njk",
  };
}
