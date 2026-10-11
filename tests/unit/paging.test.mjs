import { test } from "node:test";
import assert from "node:assert/strict";
import { PAGE_SIZE, paginate } from "../../lib/paging.js";

const items = (n) => Array.from({ length: n }, (_, i) => i + 1);

test("pages hold twelve items", () => {
  assert.equal(PAGE_SIZE, 12);
});

test("84 items make 7 full pages", () => {
  const pages = paginate(items(84), "/show-archive/");
  assert.equal(pages.length, 7);
  assert.ok(pages.every((p) => p.items.length === 12 && p.total === 7));
  assert.deepEqual(pages[6].items.slice(-1), [84]);
});

test("13 items make pages of 12 and 1", () => {
  const pages = paginate(items(13), "/x/");
  assert.deepEqual(pages.map((p) => p.items.length), [12, 1]);
});

test("an empty list still has one page", () => {
  const pages = paginate([], "/x/");
  assert.equal(pages.length, 1);
  assert.equal(pages[0].total, 1);
  assert.deepEqual(pages[0].items, []);
});

test("page 1 lives at the root and later pages under page/n/", () => {
  const pages = paginate(items(30), "/show-archive/");
  assert.deepEqual(pages.map((p) => p.href), [
    "/show-archive/",
    "/show-archive/page/2/",
    "/show-archive/page/3/",
  ]);
  assert.deepEqual(pages.map((p) => p.number), [1, 2, 3]);
});

test("pager links are null where they would not move", () => {
  const [first, middle, last] = paginate(items(30), "/r/");
  assert.equal(first.first, null);
  assert.equal(first.previous, null);
  assert.equal(first.next, "/r/page/2/");
  assert.equal(first.last, "/r/page/3/");
  assert.equal(middle.first, "/r/");
  assert.equal(middle.previous, "/r/");
  assert.equal(middle.next, "/r/page/3/");
  assert.equal(last.next, null);
  assert.equal(last.last, null);
  assert.equal(last.previous, "/r/page/2/");
});

test("a single page has no pager links", () => {
  const [only] = paginate(items(3), "/r/");
  assert.deepEqual([only.first, only.previous, only.next, only.last], [null, null, null, null]);
});
