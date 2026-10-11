import { test } from "node:test";
import assert from "node:assert/strict";
import { sanitizeNotes } from "../../lib/sanitize.js";

test("keeps the allowed formatting", () => {
  const html =
    "<h2>Links</h2><p><strong>Bold</strong> and <em>em</em><br>line</p>" +
    "<ul><li>one</li></ul><ol><li>two</li></ol><blockquote>q</blockquote><pre><code>x</code></pre>";
  assert.equal(sanitizeNotes(html), html);
});

test("removes scripts, iframes and style elements", () => {
  const out = sanitizeNotes(
    '<p>a</p><script>alert(1)</script><iframe src="https://player.example/x"></iframe><style>p{}</style>',
  );
  assert.equal(out, "<p>a</p>");
});

test("removes event handlers and style attributes", () => {
  const out = sanitizeNotes('<p onclick="x()" style="color:red">a</p><img src="https://i.example/a.png" onerror="x()">');
  assert.doesNotMatch(out, /onclick|onerror|style=/);
  assert.match(out, /<img src="https:\/\/i\.example\/a\.png"/);
});

test("allows only http, https and mailto links", () => {
  const out = sanitizeNotes(
    '<a href="javascript:alert(1)">j</a><a href="https://a.example">h</a><a href="mailto:x@example.com">m</a>',
  );
  assert.doesNotMatch(out, /javascript:/);
  assert.match(out, /href="https:\/\/a\.example"/);
  assert.match(out, /href="mailto:x@example\.com"/);
});

test("adds rel to external links", () => {
  const out = sanitizeNotes('<a href="https://a.example">h</a>');
  assert.match(out, /rel="noopener noreferrer"/);
});

test("returns an empty string for missing input", () => {
  assert.equal(sanitizeNotes(undefined), "");
});
