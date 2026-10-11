import { test } from "node:test";
import assert from "node:assert/strict";
import { normaliseKeywords, keywordSlug } from "../../lib/keywords.js";

test("normalises a comma-separated keyword list", () => {
  assert.deepEqual(normaliseKeywords(" Dev, Game Development ,, dev , App   Dev "), [
    "dev",
    "game-development",
    "app-dev",
  ]);
});

test("returns an empty list for missing input", () => {
  assert.deepEqual(normaliseKeywords(undefined), []);
  assert.deepEqual(normaliseKeywords(""), []);
});

test("slugs plain keywords unchanged", () => {
  assert.equal(keywordSlug("game-development"), "game-development");
});

test("slugs language names with symbols", () => {
  assert.equal(keywordSlug("c#"), "c-sharp");
  assert.equal(keywordSlug("c++"), "c-plus-plus");
  assert.equal(keywordSlug("f#"), "f-sharp");
});

test("replaces other unsafe characters and collapses dashes", () => {
  assert.equal(keywordSlug("node.js"), "node-js");
  assert.equal(keywordSlug("a--b//c"), "a-b-c");
  assert.equal(keywordSlug("-edge-"), "edge");
});

test("refuses a slug that would collide with archive paging", () => {
  assert.throws(() => keywordSlug("page"), /page/);
});
