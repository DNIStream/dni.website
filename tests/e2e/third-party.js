// Shared by the E2E specs that must make no third-party request: every request leaving the site's
// own origin is aborted, so artwork falls back to its placeholder and the captcha never loads.
export async function firstPartyOnly(page, baseURL) {
  const origin = new URL(baseURL).origin;
  await page.route(
    (url) => url.origin !== origin,
    (route) => route.abort(),
  );
  return origin;
}
