import { test } from "node:test";
import assert from "node:assert/strict";
import { sanitizeNotes } from "../../lib/sanitize.js";

test("keeps the allowed formatting", () => {
  const html =
    "<h2>Links</h2><p><strong>Bold</strong> and <em>em</em><br />line</p>" +
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

test("escapes bare ampersands and decodes character references in text", () => {
  assert.equal(
    sanitizeNotes("<p>Q&A, R&amp;D, &copy; &#169; &#x27; AT&T</p>"),
    "<p>Q&amp;A, R&amp;D, © © ' AT&amp;T</p>",
  );
  assert.equal(
    sanitizeNotes("<p>Tom & Jerry &abcdefghijklmnopqrstuvwxyz; end</p>"),
    "<p>Tom &amp; Jerry &amp;abcdefghijklmnopqrstuvwxyz; end</p>",
  );
});

test("keeps query-string ampersands in links, escaped once", () => {
  assert.equal(
    sanitizeNotes('<p><a href="https://a.example/?a=1&b=2&amp;c=3">q</a></p>'),
    '<p><a href="https://a.example/?a=1&amp;b=2&amp;c=3" rel="noopener noreferrer">q</a></p>',
  );
});

test("drops noscript and iframe content, including iframe fallback markup", () => {
  assert.equal(sanitizeNotes("<p>a</p><noscript><p>hidden</p></noscript><p>b</p>"), "<p>a</p><p>b</p>");
  assert.equal(
    sanitizeNotes('<p>a</p><iframe src="https://p.example"><p>fallback</p></iframe><p>b</p>'),
    "<p>a</p><p>b</p>",
  );
});

test("drops style elements inside svg", () => {
  assert.equal(sanitizeNotes("<svg><style>&lt;img src=x onerror=alert(1)&gt;</style></svg><p>b</p>"), "<p>b</p>");
});

test("never lets markup hidden behind a CDATA marker through", () => {
  const out = sanitizeNotes(
    '<p>a</p><![CDATA[<noscript><p title="</noscript><img src=x onerror=alert(1)>">]]><p>b</p>',
  );
  assert.doesNotMatch(out, /<img|onerror|<noscript/);
  assert.match(out, /^<p>a<\/p>/);
  assert.match(out, /<p>b<\/p>$/);
});
