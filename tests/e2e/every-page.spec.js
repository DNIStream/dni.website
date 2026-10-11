// Layer: E2E — constitution Principle VI: every built page is opened in a browser against the
// served image. What each page holds is proven in tests/build; this proves each one loads cleanly.
// The page list comes from dist/, which the E2E script's own build step leaves in place.
import { test, expect } from "@playwright/test";
import { pages } from "../build/helpers.mjs";
import { firstPartyOnly } from "./third-party.js";

const urls = pages.map((p) => p.url).filter((url) => url !== "/404.html");

test.describe("every page", () => {
  for (const url of urls) {
    test(url, async ({ page, baseURL }) => {
      const origin = await firstPartyOnly(page, baseURL);
      const problems = [];
      page.on("console", (msg) => {
        const from = msg.location().url;
        if (msg.type() === "error" && (!from || from.startsWith(origin))) problems.push(`console: ${msg.text()}`);
      });
      page.on("response", (response) => {
        if (response.url().startsWith(origin) && response.status() >= 400) problems.push(`${response.status()} ${response.url()}`);
      });

      const response = await page.goto(url);
      expect(response.status()).toBeLessThan(300);
      await expect(page).toHaveTitle(/^(DNI Stream|.+ \| DNI Stream)$/);
      await expect(page.locator("h1")).toHaveCount(1);
      await expect(page.getByRole("navigation", { name: "Main" })).toBeAttached();
      await expect(page.locator("footer.site-footer")).toBeAttached();

      const broken = await page.evaluate(async (own) => {
        const images = [...document.images].filter((img) => img.src.startsWith(own));
        const failed = [];
        for (const img of images) {
          img.loading = "eager";
          await img.decode().catch(() => failed.push(img.src));
        }
        return failed;
      }, origin);
      expect(broken).toEqual([]);
      expect(problems).toEqual([]);
    });
  }
});
