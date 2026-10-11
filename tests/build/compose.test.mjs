// Layer: build output — the production composition as Docker Compose itself resolves it.
import { test } from "node:test";
import assert from "node:assert/strict";
import { execFileSync } from "node:child_process";
import { readFileSync } from "node:fs";

const resolved = JSON.parse(
  execFileSync("docker", ["compose", "-f", "compose.production.yaml", "config", "--format", "json"], {
    encoding: "utf8",
    env: { ...process.env, WEBAPP_IMAGE_WEB: "example/web:0.0.0", WEBAPP_PORT_WEB: "18000" },
  }),
);
const raw = readFileSync("compose.production.yaml", "utf8");
const tracked = execFileSync("git", ["ls-files", "-z"], { encoding: "utf8" }).split("\0").filter(Boolean);

test("one service, its image and port supplied from outside, bound to loopback", () => {
  assert.deepEqual(Object.keys(resolved.services), ["web"]);
  const web = resolved.services.web;
  assert.equal(web.image, "example/web:0.0.0");
  assert.deepEqual(
    web.ports.map(({ host_ip, published, target }) => ({ host_ip, published, target })),
    [{ host_ip: "127.0.0.1", published: "18000", target: 8080 }],
  );
  assert.match(raw, /image: \$\{WEBAPP_IMAGE_WEB\}/);
  assert.match(raw, /"127\.0\.0\.1:\$\{WEBAPP_PORT_WEB\}:8080"/);
});

test("the service builds nothing and takes no environment", () => {
  for (const key of ["build", "container_name", "env_file", "environment"]) {
    assert.equal(resolved.services.web[key], undefined, key);
  }
});

test("the manifest builds the site stage with only the two build settings", () => {
  const manifest = resolved["x-webapp"];
  assert.deepEqual(manifest.images, {
    web: { build: { context: ".", dockerfile: "Dockerfile", target: "site", args: ["SITE_URL", "PLAUSIBLE_DOMAIN"] } },
  });
  assert.deepEqual(manifest.publish, ["web"]);
  assert.deepEqual(manifest.env, { required: ["SITE_URL", "PLAUSIBLE_DOMAIN"] });
  assert.equal(manifest.analytics, true);
  assert.deepEqual(manifest.services.web, {
    mem_mb: 48,
    read_only: true,
    tmpfs: ["/tmp"],
    probe: { http: "/healthz", expect: "ok", timeout: 60 },
    backup: { driver: "none" },
  });
});

test(".env.example is the only environment file, holding exactly the two build settings", () => {
  assert.deepEqual(tracked.filter((path) => /(^|\/)\.env/.test(path)), [".env.example"]);
  const keys = readFileSync(".env.example", "utf8")
    .split(/\r?\n/)
    .filter((line) => line && !line.startsWith("#"))
    .map((line) => line.split("=")[0]);
  assert.deepEqual(keys, ["SITE_URL", "PLAUSIBLE_DOMAIN"]);
});
