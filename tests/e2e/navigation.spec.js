// Layer: E2E — the menu toggle, the Listen / Subscribe disclosure and focus behaviour need a
// real browser. Link targets are data, so one page of each kind is enough.
import { test, expect } from "@playwright/test";

const SOCIAL = {
  GitHub: "https://github.com/dnistream",
  X: "https://x.com/dnistream",
  Twitch: "https://www.twitch.tv/dnistream",
  YouTube: "https://www.youtube.com/channel/UCvmKhmixJzc6Qe8mIqWn1pg",
  Discord: "https://discord.gg/KsCt2hx",
  LinkedIn: "https://www.linkedin.com/company/dnistream",
};

test.describe("navigation", () => {
  test("every kind of page has the header and footer", async ({ page }) => {
    for (const path of ["/", "/show-archive/", "/show/84-debugging/", "/nope"]) {
      await page.goto(path);
      await expect(page.getByRole("navigation", { name: "Main" }), path).toBeAttached();
      await expect(page.locator("footer.site-footer"), path).toBeVisible();
    }
  });

  test("the menu button opens and closes navigation on a phone", { tag: "@critical" }, async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 800 });
    await page.goto("/");
    const toggle = page.getByRole("button", { name: "Menu" });
    const nav = page.getByRole("navigation", { name: "Main" });
    await expect(toggle).toHaveAttribute("aria-expanded", "false");
    await expect(nav).toBeHidden();
    await toggle.click();
    await expect(toggle).toHaveAttribute("aria-expanded", "true");
    await expect(nav.getByRole("link", { name: "Podcast Archive" })).toBeVisible();
    await page.keyboard.press("Escape");
    await expect(nav).toBeHidden();
    await expect(toggle).toBeFocused();
  });

  test("Listen / Subscribe lists only operating platforms", async ({ page }) => {
    await page.goto("/");
    const listen = page.locator(".listen");
    await listen.locator("summary").click();
    const links = listen.locator(".listen__menu a.listen__platform");
    await expect(links).toHaveCount(7);
    await expect(listen.getByRole("link", { name: "RSS feed" })).toHaveAttribute("href", /\/rss$/);
    await expect(listen).not.toContainText("Google Podcasts");
    await expect(listen).not.toContainText("Stitcher");
    await expect(links.filter({ hasText: "Spotify" })).toHaveAttribute("href", /^https:\/\/open\.spotify\.com\//);
  });

  test("social links go straight to each profile", async ({ page }) => {
    await page.goto("/");
    const social = page.locator(".site-footer .social a");
    await expect(social).toHaveCount(Object.keys(SOCIAL).length);
    for (const [name, url] of Object.entries(SOCIAL)) {
      await expect(page.locator(".site-footer .social").getByRole("link", { name })).toHaveAttribute("href", url);
    }
    const hrefs = await social.evaluateAll((els) => els.map((a) => a.href));
    expect(hrefs.some((h) => /\/\/(discord|youtube)\.dnistream\./.test(h))).toBe(false);
  });
});

test.describe("navigation without JavaScript", () => {
  test.use({ javaScriptEnabled: false });

  test("shows the links and opens Listen / Subscribe on a phone", async ({ page }) => {
    await page.setViewportSize({ width: 375, height: 800 });
    await page.goto("/");
    const nav = page.getByRole("navigation", { name: "Main" });
    await expect(nav.getByRole("link", { name: "Podcast Archive" })).toBeVisible();
    await page.locator(".listen summary").click();
    await expect(page.locator(".listen__menu a").first()).toBeVisible();
  });
});
