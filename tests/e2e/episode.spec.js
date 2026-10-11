// Layer: E2E — the audio element, lazy artwork and the placeholder that stays when the podcast
// host's image fails are browser behaviours. Field mapping and sanitising are proven in unit tests.
import { test, expect } from "@playwright/test";

test.describe("episode page", () => {
  test("shows the episode with a working player", { tag: "@critical" }, async ({ page }) => {
    await page.goto("/show/84-debugging");
    await expect(page.locator("h1")).not.toBeEmpty();
    await expect(page.locator(".episode__meta time")).toHaveAttribute("datetime", /^\d{4}-/);
    await expect(page.locator(".episode__duration")).not.toBeEmpty();
    await expect(page.locator(".episode .pills a").first()).toBeVisible();
    const audio = page.locator("audio");
    await expect(audio).toHaveAttribute("controls", "");
    await expect(audio.locator("source")).toHaveAttribute("src", /^https:\/\//);
    await expect(page.getByRole("link", { name: "Download" })).toHaveAttribute("href", /^https:\/\//);
    await expect(page.getByRole("link", { name: /on Fireside/ })).toHaveAttribute("href", /^https:\/\/dnistream\.fireside\.fm\//);
    await expect(page.locator(".episode__notes")).not.toBeEmpty();
    await expect(page.locator(".episode__notes iframe, .episode__notes script")).toHaveCount(0);
  });

  test("goes back to the archive page listing the episode", async ({ page }) => {
    // Episode 84 is the newest, so page 1; "the-mvp" is an older episode further back.
    await page.goto("/show/84-debugging/");
    await expect(page.getByRole("link", { name: "Back to Archive" })).toHaveAttribute("href", "/show-archive/");
    await page.goto("/show-archive/page/3/");
    await page.locator(".episode-card a.artwork-link").first().click();
    await expect(page).toHaveURL(/\/show\/[a-z0-9-]+\/$/);
    await page.getByRole("link", { name: "Back to Archive" }).click();
    await expect(page).toHaveURL(/\/show-archive\/page\/3\/$/);
  });

  test("goes back to the keyword list it was opened from", async ({ page }) => {
    await page.goto("/show-archive/gaming/");
    await page.locator(".episode-card h2 a").first().click();
    await page.getByRole("link", { name: "Back to Archive" }).click();
    await expect(page).toHaveURL(/\/show-archive\/gaming\/$/);
  });

  test("keeps addresses without a number and without a trailing slash", async ({ page }) => {
    for (const path of ["/show/the-mvp", "/show/the-mvp/"]) {
      const response = await page.goto(path);
      expect(response.status(), path).toBe(200);
      await expect(page.locator("h1")).not.toBeEmpty();
    }
  });

  test("loads artwork lazily over a placeholder", async ({ page }) => {
    await page.goto("/show/84-debugging/");
    const img = page.locator(".episode img.artwork__img");
    await expect(img).toHaveAttribute("loading", "lazy");
    await expect(img).toHaveAttribute("decoding", "async");
    await expect(img).toHaveAttribute("width", /\d+/);
    await expect(img).toHaveAttribute("height", /\d+/);
    const background = await page.locator(".episode .artwork").evaluate((el) => getComputedStyle(el).backgroundImage);
    expect(background).toContain("episode-placeholder.svg");
  });

  test("keeps the placeholder when the artwork fails", async ({ page }) => {
    await page.route(/fireside\.fm\/file\/.*\.(jpe?g|png)/, (route) => route.abort());
    await page.goto("/show/84-debugging/");
    const img = page.locator(".episode img.artwork__img");
    await img.scrollIntoViewIfNeeded();
    await expect(img).toHaveClass(/is-failed/);
    await expect(img).toBeHidden();
    await expect(page.locator(".episode .artwork")).toBeVisible();
  });

  test("describes the episode with structured data", async ({ page }) => {
    await page.goto("/show/84-debugging/");
    const json = JSON.parse(await page.locator('script[type="application/ld+json"]').textContent());
    expect(json["@type"]).toBe("PodcastEpisode");
    expect(json.associatedMedia.contentUrl).toMatch(/^https:\/\//);
  });
});
