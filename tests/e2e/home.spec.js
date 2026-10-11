// Layer: E2E — what a first-time visitor sees on the home page, and that its links lead on.
import { test, expect } from "@playwright/test";

test.describe("home page", () => {
  test("introduces DNI and links to the podcast and the Discord", { tag: "@critical" }, async ({ page }) => {
    await page.goto("/");
    await expect(page.locator("h1")).toContainText("DNI Stream");
    const cards = page.locator(".feature-card");
    await expect(cards).toHaveCount(2);
    await expect(cards.nth(0).getByRole("link")).toHaveAttribute("href", "/show-archive/");
    await expect(cards.nth(1).getByRole("link")).toHaveAttribute("href", "https://discord.gg/KsCt2hx");
    await expect(page.locator("main")).not.toContainText("Development Now Included");
  });

  test("shows the latest episode without its date", async ({ page }) => {
    await page.goto("/");
    const latest = page.locator(".latest-episode");
    await expect(latest.getByRole("heading", { level: 2 })).toHaveText("Latest episode");
    const link = latest.locator("a.latest-episode__link");
    await expect(link).toHaveAttribute("href", /^\/show\/[a-z0-9-]+\/$/);
    await expect(link).not.toBeEmpty();
    const img = latest.locator("img.artwork__img");
    await expect(img).toHaveAttribute("loading", "lazy");
    await expect(img).toHaveAttribute("decoding", "async");
    await expect(latest.locator("time")).toHaveCount(0);
    await link.click();
    await expect(page.locator("audio")).toHaveCount(1);
  });

  test("meets the creators", async ({ page }) => {
    await page.goto("/");
    const creators = page.locator(".creator");
    await expect(creators).toHaveCount(2);
    await expect(creators.first().locator("img")).toHaveAttribute("src", /\.webp$/);
  });

  test("has no countdown and no embedded live player", async ({ page }) => {
    await page.goto("/");
    await expect(page.locator("iframe")).toHaveCount(0);
    await expect(page.getByText(/next show/i)).toHaveCount(0);
  });
});
