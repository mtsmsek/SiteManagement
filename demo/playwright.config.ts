import { defineConfig, devices } from '@playwright/test';

/**
 * Demo-recording config. One worker, one project (chromium), one spec — the
 * recorder is a single scripted run, not a test matrix. Video and trace are
 * always captured; the spec wires the recording duration to the narration's
 * length so the merged MP4 lines up frame-for-frame with the voice-over.
 */
export default defineConfig({
  testDir: './scenarios',
  fullyParallel: false,
  workers: 1,
  retries: 0,
  reporter: 'list',
  outputDir: './out/playwright',
  timeout: 5 * 60 * 1000,
  use: {
    baseURL: 'http://localhost:4200',
    viewport: { width: 1280, height: 720 },
    deviceScaleFactor: 2,
    video: {
      mode: 'on',
      size: { width: 1280, height: 720 },
    },
    headless: false,
    launchOptions: {
      slowMo: 80,
    },
  },
  projects: [
    {
      name: 'chromium',
      use: {
        ...devices['Desktop Chrome'],
        viewport: { width: 1280, height: 720 },
      },
    },
  ],
});
