// Layer: E2E — colour contrast, focus order and layout at 320 px depend on computed styles in a
// real browser, as does what a page writes to cookies and storage.
import { test, expect } from "@playwright/test";
import AxeBuilder from "@axe-core/playwright";
import { firstPartyOnly } from "./third-party.js";

const PAGES = ["/", "/show-archive/", "/show-archive/dev/", "/show/84-debugging/", "/contact/", "/privacy/", "/no-such-page"];
const WCAG = ["wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22aa"];

for (const scheme of ["dark", "light"]) {
  test.describe(`accessibility, ${scheme} scheme`, () => {
    test.use({ colorScheme: scheme });

    for (const path of PAGES) {
      test(`${path} has no WCAG violations`, async ({ page, baseURL }) => {
        await firstPartyOnly(page, baseURL);
        await page.goto(path);
        const { violations } = await new AxeBuilder({ page }).withTags(WCAG).analyze();
        expect(violations.map((v) => `${v.id}: ${v.nodes.map((n) => n.target.join(" ")).join(", ")}`)).toEqual([]);
      });
    }
  });
}

test.describe("at 320 px, without storage", () => {
  test.use({ viewport: { width: 320, height: 700 } });

  test("no page scrolls sideways or stores anything", async ({ page, context, baseURL }) => {
    await firstPartyOnly(page, baseURL);
    for (const path of PAGES) {
      await page.goto(path);
      const overflow = await page.evaluate(() => document.documentElement.scrollWidth - document.documentElement.clientWidth);
      expect(overflow, `${path} overflow`).toBeLessThanOrEqual(0);
      const stored = await page.evaluate(() => localStorage.length + sessionStorage.length);
      expect(stored, `${path} storage`).toBe(0);
    }
    expect(await context.cookies()).toEqual([]);
  });
});
