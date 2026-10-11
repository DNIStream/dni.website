// Layer: build output — the budget and the hosts a page loads are facts of the built files,
// checked without a browser.
import { test } from "node:test";
import assert from "node:assert/strict";
import { pages, attrs, stylesheets, fileSize, isLocal } from "./helpers.mjs";

const BUDGET = 30 * 1024;
const SCRIPT_HOSTS = new Set(["web3forms.com", "hcaptcha.com"]);
const allowedScriptHost = (host) =>
  [...SCRIPT_HOSTS].some((allowed) => host === allowed || host.endsWith(`.${allowed}`));

test("the build produced pages", () => {
  assert.ok(pages.length > 0);
});

test("every page's first-party CSS and JS stays within 30 KB", () => {
  for (const { url, html } of pages) {
    const assets = new Set([...stylesheets(html), ...attrs(html, "script", "src")].filter(isLocal));
    const total = [...assets].reduce((sum, path) => sum + fileSize(path), 0);
    assert.ok(total <= BUDGET, `${url}: ${total} bytes of first-party CSS and JS`);
  }
});

test("pages load scripts and styles only from the site and the hosts the policy allows", () => {
  for (const { url, html } of pages) {
    for (const src of attrs(html, "script", "src").filter((s) => !isLocal(s))) {
      assert.ok(allowedScriptHost(new URL(src).hostname), `${url}: script from ${src}`);
    }
    for (const href of stylesheets(html)) {
      assert.ok(isLocal(href), `${url}: stylesheet from ${href}`);
    }
    for (const action of attrs(html, "form", "action")) {
      assert.equal(new URL(action).hostname, "api.web3forms.com", `${url}: form posts to ${action}`);
    }
  }
});

test("no page contains an iframe", () => {
  for (const { url, html } of pages) {
    assert.doesNotMatch(html, /<iframe\b/i, `${url} has an iframe`);
  }
});

test("no page links to the old API", () => {
  for (const { url, html } of pages) {
    assert.ok(!attrs(html, "a", "href").some((h) => /^\/api(\/|$)/.test(h)), `${url} links to /api/`);
  }
});
