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
