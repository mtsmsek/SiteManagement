/**
 * Single source of truth for the demo scenario: the ordered list of scenes
 * the recorder walks through. Each scene's <c>narration</c> is fed to
 * piper-tts to produce the matching audio clip; the merge step splits the
 * same narration into short chunks (see <c>lib/split.ts</c>) so the on-screen
 * subtitles flow with the voice-over instead of dumping the whole sentence
 * at once. The recording stage holds the UI on each scene for at least
 * <c>minDurationMs</c>, which keeps audio and frames lined up after merge.
 */
export interface Scene {
  /** Stable id, used as the audio filename and as the recorder-stage hook. */
  id: string;
  /** Short human-readable label printed to the console during recording. */
  label: string;
  /** Sentence(s) read by piper-tts as the voice-over for this scene. */
  narration: string;
  /**
   * Minimum on-screen duration. The recorder waits at least this long on the
   * scene even if the click + assert it just performed finishes faster, so
   * the narration audio has the room it needs after the merge.
   */
  minDurationMs: number;
}

export const scenes: Scene[] = [
  {
    id: '01-intro',
    label: 'Intro: project overview',
    narration:
      'SiteManagement is an apartment complex administration platform built in dot net 10 with Domain-Driven Design, plus a separate payment microservice on MongoDB and an Angular 21 front-end.',
    minDurationMs: 8000,
  },
  {
    id: '02-resident-login',
    label: 'Resident logs in',
    narration:
      'The first scene: a resident logs into their own self-service portal. Authentication is JWT, the password came in by email from the demo seeder.',
    minDurationMs: 6000,
  },
  {
    id: '03-resident-dashboard',
    label: 'Resident dashboard',
    narration:
      'The dashboard shows the resident their outstanding balance, their credit, and unread messages. Three tiles, three direct links, no clutter.',
    minDurationMs: 7000,
  },
  {
    id: '04-resident-bills',
    label: 'Resident bills page',
    narration:
      'On the bills page every dues and utility item the resident owns is listed. One item is already settled, two are still due.',
    minDurationMs: 6000,
  },
  {
    id: '05-pay-by-card-open',
    label: 'Open pay-by-card dialog',
    narration:
      'The resident picks an unpaid line and opens the pay-by-card dialog. The card details ride over Refit + Polly to the payment microservice; idempotency is on the line item id.',
    minDurationMs: 7000,
  },
  {
    id: '06-pay-by-card-success',
    label: 'Pay-by-card success',
    narration:
      'The charge succeeds. The item flips to paid in the same transaction; if the charge had been declined the item would stay unpaid and the API would return a 402.',
    minDurationMs: 7000,
  },
  {
    id: '07-resident-messaging',
    label: 'Resident messaging',
    narration:
      'On the messaging page the resident has one thread from the admin, with an unread badge. Opening the thread clears the badge and marks the admin messages read on the server.',
    minDurationMs: 7000,
  },
  {
    id: '08-resident-reply',
    label: 'Resident sends a reply',
    narration:
      'The resident sends a reply. The same message will land on the admin side in real time through SignalR — push only, no polling.',
    minDurationMs: 7000,
  },
  {
    id: '09-admin-login',
    label: 'Admin logs in',
    narration:
      'Now the admin side. The same login endpoint, the same JWT pipeline, a different role marker. Authorization is a MediatR pipeline concern — every request declares exactly one role marker or the build fails.',
    minDurationMs: 8000,
  },
  {
    id: '10-admin-dashboard',
    label: 'Admin dashboard',
    narration:
      'The admin dashboard rolls up the whole site: residents, accrued and collected totals, outstanding balance, collection rate. Pure read-side projection — no domain entities cross the boundary.',
    minDurationMs: 8000,
  },
  {
    id: '11-admin-messaging',
    label: 'Admin messaging — sees the resident reply',
    narration:
      'On the admin messaging page the reply the resident just sent is already there, no refresh needed. That is the SignalR push at work.',
    minDurationMs: 7000,
  },
  {
    id: '12-outro',
    label: 'Outro',
    narration:
      'Ten architecture decision records, 90 percent line coverage, three hundred and seventy-seven backend tests plus 32 web tests. MIT licensed and open source on GitHub.',
    minDurationMs: 8000,
  },
];

/**
 * Resident used by the recording. The seeder marks the first resident's item
 * as already paid, so we drive the demo through the second resident — the
 * pay-by-card scene needs an unpaid line to click.
 */
export const recordingResident = {
  email: 'mert.kaya@demo.local',
  displayName: 'Mert Kaya',
};

/** Bootstrap admin credentials (read from the dev env; consistent with .env.example). */
export const recordingAdmin = {
  email: 'admin@sitemanagement.local',
  password: 'Str0ng-P@ss-Dev',
};

/** Demo card seeded by PaymentService — Luhn-valid with funds for the charge. */
export const demoCard = {
  number: '4242 4242 4242 4242',
  cvv: '123',
  expiryMonth: '12',
  expiryYear: '2030',
};
