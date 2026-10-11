// Layer: E2E (HTTP against the running production image) — headers, compression, caching and
// status codes come from nginx's configuration, which only the real container exercises.
import { test, expect } from "@playwright/test";

const SECURITY_HEADERS = {
  "x-content-type-options": "nosniff",
  "x-frame-options": "DENY",
  "referrer-policy": "strict-origin-when-cross-origin",
};

function expectSecurityHeaders(response) {
  const headers = response.headers();
  for (const [name, value] of Object.entries(SECURITY_HEADERS)) {
    expect(headers[name], name).toBe(value);
  }
  expect(headers["permissions-policy"]).toContain("camera=()");
  const csp = headers["content-security-policy"];
  expect(csp).toContain("default-src 'self'");
  expect(csp).toContain("form-action https://api.web3forms.com");
  expect(csp).toContain("frame-ancestors 'none'");
}

test.describe("production image", { tag: "@critical" }, () => {
  test("answers the health check without caching", async ({ request }) => {
    const response = await request.get("/healthz");
    expect(response.status()).toBe(200);
    expect((await response.text()).trim()).toBe("ok");
    expect(response.headers()["cache-control"]).toBe("no-store");
  });

  test("serves HTML uncached, with security headers", async ({ request }) => {
    const response = await request.get("/");
    expect(response.status()).toBe(200);
    expect(response.headers()["cache-control"]).toBe("no-cache");
    expectSecurityHeaders(response);
  });

  test("serves CSS cached long-term, compressed, with security headers", async ({ request }) => {
    const html = await (await request.get("/")).text();
    const href = html.match(/href="(\/assets\/css\/site\.css\?v=[^"]+)"/)[1];
    const response = await request.get(href, { headers: { "accept-encoding": "gzip" } });
    expect(response.status()).toBe(200);
    expect(response.headers()["cache-control"]).toBe("public, max-age=31536000, immutable");
    expect(response.headers()["content-encoding"]).toBe("gzip");
    expectSecurityHeaders(response);
  });

  test("caches images for a day", async ({ request }) => {
    const response = await request.get("/assets/img/logo.svg");
    expect(response.status()).toBe(200);
    expect(response.headers()["cache-control"]).toBe("public, max-age=86400");
  });

  for (const path of ["/nope", "/api/shows"]) {
    test(`answers ${path} with the 404 page`, async ({ request }) => {
      const response = await request.get(path);
      expect(response.status()).toBe(404);
      expect(await response.text()).toContain("Page not found");
      expectSecurityHeaders(response);
    });
  }
});
