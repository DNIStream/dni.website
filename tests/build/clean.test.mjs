// Layer: build output — what the repository tracks is a fact of the checkout, read with git.
import { test } from "node:test";
import assert from "node:assert/strict";
import { execFileSync } from "node:child_process";
import { readFileSync } from "node:fs";

const tracked = execFileSync("git", ["ls-files", "-z"], { encoding: "utf8" }).split("\0").filter(Boolean);

const RETIRED = [
  /^src\//,
  /^test\//,
  /^nginx\//,
  /^DNI\.sln$/,
  /^nuget\.config$/,
  /^(run-service-hosts|setup-env-vars|version)\.ps1$/,
  /^deploy\.sh$/,
  /^docker-compose\.yml$/,
  /^\.dockerignore$/,
  /^\.github\/workflows\/(ci|cd)\.yml$/,
];

// The site's own sources, scripts and configuration. Specs and tooling describe the work and are
// checked by review; the feed snapshot is the podcast host's published feed, kept verbatim.
const CHECKED = (path) =>
  !/^(specs|\.specify|\.claude)\//.test(path) &&
  path !== "site/_data/feed-snapshot.xml" &&
  path !== "tests/build/clean.test.mjs" &&
  !/\.(png|ico|webp|woff2|jpe?g|svg)$/.test(path);

const HOSTING = [
  ["an IP address other than loopback", /\b(?!127\.0\.0\.1\b)(?:\d{1,3}\.){3}\d{1,3}\b/],
  ["a proxy directive", /\bproxy_pass\b|\bserver_name\b/],
  ["a TLS certificate path", /ssl_certificate|letsencrypt|\.pem\b/i],
  ["a remote shell step", /\b(?:ssh|scp|rsync)\s/],
  ["a registry login or push", /docker\s+(?:login|push)|ghcr\.io|docker\.io\//],
  ["a deployment step", /\bdeploy(?:ment|ing|ed|s)?\b/i],
  ["a credential", /\b(?:password|passwd|secret|api[_-]?token)\s*[:=]/i],
  ["the old mail server", /\bSMTP_/],
];

test("nothing under the retired paths is tracked", () => {
  const left = tracked.filter((path) => RETIRED.some((re) => re.test(path)));
  assert.deepEqual(left, []);
});

test("no tracked env file other than the example", () => {
  assert.deepEqual(tracked.filter((path) => /(^|\/)\.env(\.|$)/.test(path) && path !== ".env.example"), []);
});

test("no tracked file carries hosting detail", () => {
  const found = [];
  for (const path of tracked.filter(CHECKED)) {
    const text = readFileSync(path, "utf8");
    for (const [what, re] of HOSTING) if (re.test(text)) found.push(`${path}: ${what}`);
  }
  assert.deepEqual(found, []);
});
