// Layer: E2E — the form's enhanced submit, its live status and the in-flight guard run in the
// browser against the form service's HTTP contract. The service and its captcha are stubbed, so
// no message is ever sent; real delivery is a manual check.
import { test, expect } from "@playwright/test";

const SUBMIT = "https://api.web3forms.com/submit";

// Stands in for the form service's captcha script: renders the response field the real widget adds.
const CAPTCHA_STUB = `
  window.hcaptcha = { reset: function () { window.__captchaResets = (window.__captchaResets || 0) + 1; } };
  document.querySelectorAll(".h-captcha").forEach(function (el) {
    var field = document.createElement("textarea");
    field.name = "h-captcha-response";
    field.hidden = true;
    el.appendChild(field);
  });`;

async function openContact(page, onSubmit) {
  const requests = [];
  await page.route("https://web3forms.com/client/script.js", (route) =>
    route.fulfill({ contentType: "application/javascript", body: CAPTCHA_STUB }),
  );
  await page.route(SUBMIT, async (route) => {
    requests.push(route.request());
    await onSubmit(route);
  });
  await page.goto("/contact");
  return requests;
}

const passCaptcha = (page) =>
  page.locator('[name="h-captcha-response"]').evaluate((el) => {
    el.value = "token";
  });

async function fill(page) {
  await page.getByLabel("Name").fill("Ada");
  await page.getByLabel("Email").fill("ada@example.com");
  await page.getByLabel("Message").fill("Hello DNI");
}

const ok = (route) =>
  route.fulfill({ contentType: "application/json", body: JSON.stringify({ success: true, message: "sent" }) });

test.describe("contact form", () => {
  test("has the fields and hidden settings the form service needs", async ({ page }) => {
    await openContact(page, ok);
    const form = page.locator("form.contact-form");
    await expect(form).toHaveAttribute("action", SUBMIT);
    await expect(form).toHaveAttribute("method", "post");
    await expect(page.getByLabel("Name")).toHaveAttribute("required", "");
    await expect(page.getByLabel("Email")).toHaveAttribute("type", "email");
    await expect(page.getByLabel("Email")).toHaveAttribute("required", "");
    const message = page.getByLabel("Message");
    await expect(message).toHaveAttribute("required", "");
    await expect(message).toHaveAttribute("maxlength", "5000");
    const hint = page.locator(`#${await message.getAttribute("aria-describedby")}`);
    await expect(hint).toContainText("5,000 characters");
    await expect(form.locator('input[name="access_key"]')).toHaveAttribute("value", /^[0-9a-f-]{36}$/);
    await expect(form.locator('input[name="subject"]')).toHaveAttribute("value", "New DNI Stream contact message");
    await expect(form.locator('input[name="from_name"]')).toHaveAttribute("value", "DNI Stream Website");
  });

  test("hides the honeypot from people and assistive technology", async ({ page }) => {
    await openContact(page, ok);
    const honeypot = page.locator('input[name="botcheck"]');
    await expect(honeypot).toHaveAttribute("tabindex", "-1");
    await expect(honeypot).toHaveAttribute("aria-hidden", "true");
    await expect(honeypot).not.toBeInViewport();
    await expect(honeypot).not.toBeChecked();
  });

  test("sends a message and confirms it", { tag: "@critical" }, async ({ page }) => {
    const requests = await openContact(page, ok);
    await fill(page);
    await passCaptcha(page);
    await page.getByRole("button", { name: "Send" }).click();
    await expect(page.getByRole("status")).toHaveText("Thanks — your message is on its way. We'll be in touch.");
    await expect(page.getByLabel("Name")).toHaveValue("");
    expect(requests).toHaveLength(1);
    expect(requests[0].headers()["accept"]).toBe("application/json");
    expect(await page.evaluate(() => window.__captchaResets)).toBe(1);
  });

  test("asks for the human check and sends nothing without it", async ({ page }) => {
    const requests = await openContact(page, ok);
    await fill(page);
    await page.getByRole("button", { name: "Send" }).click();
    await expect(page.getByRole("status")).toHaveText('Please complete the "I am human" check before sending.');
    expect(requests).toHaveLength(0);
  });

  test("lets the browser report a missing field", async ({ page }) => {
    const requests = await openContact(page, ok);
    await page.getByLabel("Email").fill("ada@example.com");
    await passCaptcha(page);
    await page.getByRole("button", { name: "Send" }).click();
    expect(await page.getByLabel("Name").evaluate((el) => el.validity.valueMissing)).toBe(true);
    expect(requests).toHaveLength(0);
  });

  test("shows the service's error and keeps what was typed", async ({ page }) => {
    await openContact(page, (route) =>
      route.fulfill({ status: 400, contentType: "application/json", body: JSON.stringify({ success: false, message: "Invalid access key" }) }),
    );
    await fill(page);
    await passCaptcha(page);
    await page.getByRole("button", { name: "Send" }).click();
    await expect(page.getByRole("status")).toHaveText("Invalid access key");
    await expect(page.getByLabel("Message")).toHaveValue("Hello DNI");
    await expect(page.getByRole("button", { name: "Send" })).toBeEnabled();
  });

  test("reports a network failure and keeps what was typed", async ({ page }) => {
    await openContact(page, (route) => route.abort());
    await fill(page);
    await passCaptcha(page);
    await page.getByRole("button", { name: "Send" }).click();
    await expect(page.getByRole("status")).toHaveText("Network error — please try again in a moment.");
    await expect(page.getByLabel("Name")).toHaveValue("Ada");
  });

  test("sends once when Send is pressed twice", async ({ page }) => {
    let release;
    const held = new Promise((resolve) => (release = resolve));
    const requests = await openContact(page, async (route) => {
      await held;
      await ok(route);
    });
    await fill(page);
    await passCaptcha(page);
    const send = page.getByRole("button", { name: "Send" });
    await send.click();
    await expect(page.getByRole("status")).toHaveText("Sending…");
    await expect(send).toBeDisabled();
    await send.click({ force: true });
    release();
    await expect(page.getByRole("status")).toContainText("on its way");
    expect(requests).toHaveLength(1);
  });

  test("offers Discord and social profiles, with no embedded frames", async ({ page }) => {
    await openContact(page, ok);
    await expect(page.locator("main").getByRole("link", { name: /Discord/ }).first()).toHaveAttribute("href", "https://discord.gg/KsCt2hx");
    await expect(page.locator("main .social a")).toHaveCount(6);
    await expect(page.locator("iframe")).toHaveCount(0);
  });
});
