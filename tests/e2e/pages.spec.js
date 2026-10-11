// Layer: E2E — the served status codes and the 404 route are nginx behaviour, observable only
// against the running image. Page inventory and metadata are proven in tests/build.
import { test, expect } from "@playwright/test";

const PAGES = [
  ["/faq", "FAQ | DNI Stream", "Frequently asked questions"],
  ["/privacy", "Privacy & Cookies | DNI Stream", "Privacy and cookie policy"],
  ["/ethics", "Code of Ethics | DNI Stream", "Code of ethics"],
  ["/community-guidelines", "Community Guidelines | DNI Stream", "Community guidelines"],
];

test.describe("policy pages", () => {
  for (const [path, title, heading] of PAGES) {
    test(`${path} answers with its title and heading`, async ({ page }) => {
      const response = await page.goto(path);
      expect(response.status()).toBe(200);
      await expect(page).toHaveTitle(title);
      await expect(page.locator("h1")).toHaveText(heading);
    });
  }

  test("the privacy page names today's services and none of the old ones", async ({ page }) => {
    await page.goto("/privacy");
    const text = page.locator("main");
    for (const name of ["Web3Forms", "hCaptcha", "Fireside", "Plausible"]) await expect(text).toContainText(name);
    for (const name of ["Google Analytics", "reCAPTCHA", "SMTP"]) await expect(text).not.toContainText(name);
  });

  test("an unknown address gets the 404 page with a link home", async ({ page }) => {
    const response = await page.goto("/no-such-page");
    expect(response.status()).toBe(404);
    await expect(page.locator("h1")).toHaveText("Page not found");
    await expect(page.locator("main").getByRole("link", { name: /home page/i })).toHaveAttribute("href", "/");
  });
});
