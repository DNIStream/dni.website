// Layer: E2E — the grid's column count, paging through the served pages and behaviour without
// JavaScript can only be seen in a real browser. Which pages exist is proven in tests/build.
import { test, expect } from "@playwright/test";

const cards = (page) => page.locator(".episode-grid > li");

test.describe("podcast archive", () => {
  test("lists twelve episodes newest first with their details", { tag: "@critical" }, async ({ page }) => {
    await page.goto("/show-archive");
    await expect(page.locator("h1")).toHaveText("Podcast Archive");
    await expect(cards(page)).toHaveCount(12);
    const first = cards(page).first();
    await expect(first.locator("h2 a")).toHaveAttribute("href", /^\/show\/[a-z0-9-]+\/$/);
    await expect(first.locator("img.artwork__img")).toHaveAttribute("loading", "lazy");
    await expect(first.locator(".episode-card__summary")).not.toBeEmpty();
    await expect(first.locator("time")).toHaveAttribute("datetime", /^\d{4}-\d{2}-\d{2}/);
    await expect(first.locator(".episode-card__duration")).not.toBeEmpty();
    await expect(first.locator(".pills a").first()).toHaveAttribute("href", /^\/show-archive\/[a-z0-9-]+\/$/);

    const dates = await cards(page).locator("time").evaluateAll((els) => els.map((e) => e.dateTime));
    expect([...dates].sort().reverse()).toEqual(dates);
  });

  test("pages through to the last page and back", async ({ page }) => {
    await page.goto("/show-archive/");
    const pager = page.getByRole("navigation", { name: "Pages" });
    await expect(pager.getByRole("link", { name: "Previous page" })).toHaveCount(0);
    await pager.getByRole("link", { name: "Last page" }).click();
    await expect(page).toHaveURL(/\/show-archive\/page\/7\/$/);
    await expect(page.locator("h1")).toContainText("page 7");
    await expect(pager.getByRole("link", { name: "Next page" })).toHaveCount(0);
    await pager.getByRole("link", { name: "Previous page" }).click();
    await expect(page).toHaveURL(/\/show-archive\/page\/6\/$/);
    await pager.getByRole("link", { name: "First page" }).click();
    await expect(page).toHaveURL(/\/show-archive\/$/);
  });

  test("shows the ten most used keywords with their counts", async ({ page }) => {
    await page.goto("/show-archive/");
    const top = page.locator(".top-keywords li");
    await expect(top).toHaveCount(10);
    await expect(top.first()).toContainText("dev");
    await expect(top.first()).toContainText("24");
  });

  test("filters by a keyword", async ({ page }) => {
    await page.goto("/show-archive/");
    await page.locator(".top-keywords a", { hasText: "gaming" }).click();
    await expect(page).toHaveURL(/\/show-archive\/gaming\/$/);
    await expect(page.locator("h1")).toContainText("gaming");
    // Nine episodes carry "gaming" in the committed feed snapshot.
    await expect(cards(page)).toHaveCount(9);
    for (const card of await cards(page).all()) {
      await expect(card.locator(".pills")).toContainText("gaming");
    }
  });

  test("lays the grid out in 1 to 4 columns", { tag: "@extended" }, async ({ page }) => {
    for (const [width, columns] of [[375, 1], [768, 2], [1024, 3], [1440, 4]]) {
      await page.setViewportSize({ width, height: 900 });
      await page.goto("/show-archive/");
      const count = await page
        .locator(".episode-grid")
        .evaluate((el) => getComputedStyle(el).gridTemplateColumns.split(" ").length);
      expect(count, `${width}px`).toBe(columns);
    }
  });
});

test.describe("podcast archive without JavaScript", () => {
  test.use({ javaScriptEnabled: false });

  test("pages and filters", async ({ page }) => {
    await page.goto("/show-archive/");
    await page.getByRole("navigation", { name: "Pages" }).getByRole("link", { name: "Next page" }).click();
    await expect(page).toHaveURL(/\/show-archive\/page\/2\/$/);
    await expect(cards(page)).toHaveCount(12);
    await page.locator(".top-keywords a", { hasText: "dev" }).first().click();
    await expect(page.locator("h1")).toContainText("dev");
  });
});
