// Layer: build output — the version is read from VERSION at build time and only exists once the footer is rendered.
import { test } from "node:test";
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { pages } from "./helpers.mjs";

const version = readFileSync("VERSION", "utf8").trim();

test("every page's footer shows the site version from VERSION", () => {
  assert.match(version, /^\d+\.\d+\.\d+$/);
  for (const p of pages) {
    const footer = p.html.match(/<footer[\s\S]*?<\/footer>/)?.[0];
    assert.ok(footer, `${p.url} footer`);
    assert.ok(footer.includes(`v${version}`), `${p.url} footer shows v${version}`);
  }
});
