import { Page, test, expect } from '@playwright/test';
import { mkdirSync, writeFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { readWelcomePasswordFor } from '../lib/mailhog.js';
import { demoCard, recordingAdmin, recordingResident, scenes } from '../lib/scenes.js';

/**
 * The one scenario the demo recorder runs. Each scene drives the UI through
 * one small slice, then waits at least the scene's minDuration so the
 * narration that piper-tts produces fits the on-screen action. The recorded
 * video lands in demo/out/playwright/.../video.webm; the orchestrator
 * (scripts/record-demo.ps1) then runs piper-tts + ffmpeg to attach audio
 * and produce docs/demo-video.mp4.
 *
 * The Angular UI is forced into English via the localStorage key the
 * LanguageService reads on bootstrap, so the captured frames match the
 * English narration.
 */
test.use({
  storageState: {
    cookies: [],
    origins: [
      {
        origin: 'http://localhost:4200',
        localStorage: [{ name: 'sm.lang', value: 'en' }],
      },
    ],
  },
});

test('records the SiteManagement demo', async ({ page }, testInfo) => {
  testInfo.setTimeout(5 * 60 * 1000);

  // 01 — intro
  await openScene(page, '01-intro');
  await page.goto('/login');
  await expect(page.locator('h1.login-brand__title')).toBeVisible();
  await holdScene(page, '01-intro');

  // 02 — resident login
  await openScene(page, '02-resident-login');
  const residentPassword = await readWelcomePasswordFor(recordingResident.email);
  await loginAs(page, recordingResident.email, residentPassword);
  await page.waitForURL('**/resident/dashboard', { timeout: 20_000 });
  await holdScene(page, '02-resident-login');

  // 03 — resident dashboard
  await openScene(page, '03-resident-dashboard');
  await expect(page.locator('.tile').first()).toBeVisible();
  await holdScene(page, '03-resident-dashboard');

  // 04 — bills
  await openScene(page, '04-resident-bills');
  await page.locator('mat-nav-list a[routerLink="/resident/bills"]').click();
  await expect(page.locator('table.data-table')).toBeVisible();
  await holdScene(page, '04-resident-bills');

  // 05 — open pay-by-card
  await openScene(page, '05-pay-by-card-open');
  await page.locator('table.data-table button:has-text("Pay by card")').first().click();
  await expect(page.locator('h2[mat-dialog-title]')).toBeVisible();
  await holdScene(page, '05-pay-by-card-open');

  // 06 — fill card + submit, expect the line to flip to "Paid"
  await openScene(page, '06-pay-by-card-success');
  await page
    .locator('input[formcontrolname="cardNumber"]')
    .pressSequentially(demoCard.number.replace(/\s/g, ''), { delay: KEYSTROKE_DELAY_MS });
  await page
    .locator('input[formcontrolname="cvv"]')
    .pressSequentially(demoCard.cvv, { delay: KEYSTROKE_DELAY_MS });
  await selectMatOption(page, 'expiryMonth', demoCard.expiryMonth);
  await selectMatOption(page, 'expiryYear', demoCard.expiryYear);
  await page.waitForTimeout(500);
  await page.locator('mat-dialog-actions button[type="submit"]').click();
  await expect(page.locator('h2[mat-dialog-title]')).not.toBeVisible({ timeout: 15_000 });
  await holdScene(page, '06-pay-by-card-success');

  // 07 — messaging
  await openScene(page, '07-resident-messaging');
  await page.locator('mat-nav-list a[routerLink="/resident/messages"]').click();
  const firstThread = page.locator('button.thread-item').first();
  await expect(firstThread).toBeVisible();
  await firstThread.click();
  await expect(page.locator('.bubble').first()).toBeVisible();
  await holdScene(page, '07-resident-messaging');

  // 08 — resident reply
  await openScene(page, '08-resident-reply');
  const replyBox = page.locator('.reply textarea');
  await replyBox.click();
  await replyBox.pressSequentially('Thanks for the reminder, paying it now.', {
    delay: KEYSTROKE_DELAY_MS,
  });
  await page.waitForTimeout(600);
  await page.locator('.reply button[matButton="filled"]').click();
  await expect(page.locator('.bubble').last()).toContainText('Thanks for the reminder', { timeout: 10_000 });
  await holdScene(page, '08-resident-reply');

  // 09 — admin login
  await openScene(page, '09-admin-login');
  // Logout drops back to /login.
  await page.locator('button:has-text("Log out")').click();
  await page.waitForURL('**/login', { timeout: 10_000 });
  await loginAs(page, recordingAdmin.email, recordingAdmin.password);
  await page.waitForURL('**/admin/dashboard', { timeout: 20_000 });
  await holdScene(page, '09-admin-login');

  // 10 — admin dashboard
  await openScene(page, '10-admin-dashboard');
  await expect(page.locator('.tile').first()).toBeVisible();
  await holdScene(page, '10-admin-dashboard');

  // 11 — admin messaging
  await openScene(page, '11-admin-messaging');
  await page.locator('mat-nav-list a[routerLink="/admin/messaging"]').click();
  const adminFirstThread = page.locator('button.thread-item').first();
  await expect(adminFirstThread).toBeVisible();
  await adminFirstThread.click();
  await expect(page.locator('.bubble').last()).toContainText('Thanks for the reminder', { timeout: 15_000 });
  await holdScene(page, '11-admin-messaging');

  // 12 — outro: stay on the admin messaging page, just hold while the narration runs
  await openScene(page, '12-outro');
  await holdScene(page, '12-outro');

  // Persist the scene timings so the merge step lines audio up to video.
  await writeManifest(testInfo.outputDir);
});

/** Bookkeeping shared by the holds — keeps the recorder's console useful while a scene is on screen. */
async function openScene(page: Page, sceneId: string): Promise<void> {
  const scene = scenes.find((s) => s.id === sceneId)!;
  const stamp = Date.now();
  recordSceneStart(sceneId, stamp);
  console.log(`▶ ${sceneId} — ${scene.label}`);
}

/** Holds the current scene on-screen for at least its minDurationMs. */
async function holdScene(page: Page, sceneId: string): Promise<void> {
  const scene = scenes.find((s) => s.id === sceneId)!;
  const start = sceneStarts.get(sceneId)!;
  const elapsed = Date.now() - start;
  const remaining = Math.max(0, scene.minDurationMs - elapsed);
  if (remaining > 0) {
    await page.waitForTimeout(remaining);
  }
  recordSceneEnd(sceneId, Date.now());
}

/** Picks an option from a closed Material select by its visible label. */
async function selectMatOption(page: Page, formControlName: string, value: string): Promise<void> {
  await page.locator(`mat-select[formcontrolname="${formControlName}"]`).click();
  await page.locator(`mat-option:has-text("${value}")`).first().click();
}

/** Per-character typing delay used everywhere a viewer is meant to follow the input. */
const KEYSTROKE_DELAY_MS = 95;

/**
 * Logs in through the Material login form. Reactive forms can drop a click on
 * the submit button if the input hasn't blurred, so we type the credentials
 * with a tab between fields and finish with Enter on the password — that
 * triggers ngSubmit reliably and matches how a real keyboard user works.
 * Inputs are typed character-by-character with KEYSTROKE_DELAY_MS so the
 * viewer can actually read what's being entered.
 */
async function loginAs(page: Page, email: string, password: string): Promise<void> {
  const emailInput = page.locator('input[formcontrolname="email"]');
  await emailInput.click();
  await emailInput.pressSequentially(email, { delay: KEYSTROKE_DELAY_MS });
  await emailInput.press('Tab');

  const passwordInput = page.locator('input[formcontrolname="password"]');
  await passwordInput.pressSequentially(password, { delay: KEYSTROKE_DELAY_MS });
  await page.waitForTimeout(600);
  await passwordInput.press('Enter');
}

/** Scene-timing manifest, consumed by the merge step. */
interface SceneTiming {
  id: string;
  startMs: number;
  endMs: number;
}
const sceneStarts = new Map<string, number>();
const sceneTimings: SceneTiming[] = [];
let recordStartedAt = 0;

function recordSceneStart(id: string, atEpochMs: number): void {
  if (recordStartedAt === 0) {
    recordStartedAt = atEpochMs;
  }
  sceneStarts.set(id, atEpochMs);
}

function recordSceneEnd(id: string, atEpochMs: number): void {
  const start = sceneStarts.get(id)!;
  sceneTimings.push({
    id,
    startMs: start - recordStartedAt,
    endMs: atEpochMs - recordStartedAt,
  });
}

function writeManifest(outputDir: string): void {
  const manifestDir = resolve(outputDir, '..');
  mkdirSync(manifestDir, { recursive: true });
  const manifestPath = resolve(manifestDir, 'scene-manifest.json');
  writeFileSync(manifestPath, JSON.stringify({ recordStartedAt, scenes: sceneTimings }, null, 2), 'utf-8');
  console.log(`✓ manifest written: ${manifestPath}`);
}
