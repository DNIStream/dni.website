import { defineConfig, devices } from "@playwright/test";

export default defineConfig({
  testDir: "tests/e2e",
  workers: 2,
  retries: 1,
  reporter: "list",
  use: {
    baseURL: process.env.BASE_URL || "http://localhost:8089",
    trace: "on-first-retry",
  },
  projects: [{ name: "chromium", use: { ...devices["Desktop Chrome"] } }],
});
