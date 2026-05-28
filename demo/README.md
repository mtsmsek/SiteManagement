# Demo recorder

Single-command, fully open-source pipeline that records the SiteManagement
showcase video end-to-end. No cloud TTS, no API keys, no subscriptions —
Playwright drives the UI, [piper-tts](https://github.com/rhasspy/piper)
(open source, offline) reads the English narration, and
[ffmpeg-static](https://www.npmjs.com/package/ffmpeg-static) merges them.

## Prerequisites

- Docker Desktop running
- Node 20+ and npm
- Python 3.10+ with `piper-tts` installed: `pip install piper-tts`
- Angular dev server running in another terminal: `cd web && npm start`

The first run pulls the Playwright Chromium build and the
`en_US-lessac-medium` piper voice (~60 MB) from HuggingFace; both land
under `demo/` and are git-ignored.

## Run it

From the repo root, in PowerShell:

```powershell
.\scripts\record-demo.ps1
```

The orchestrator:

1. Wipes Docker volumes and brings the stack up with `DemoSeeder` on (clean
   site + 3 residents + open dues period + two admin-opened threads).
2. Waits for the API, MailHog, and the Angular dev server to be reachable.
3. Runs the Playwright scenario (`scenarios/demo.spec.ts`) — twelve scenes,
   captured as a WebM in `demo/out/playwright/`, plus a scene-timing
   manifest.
4. Renders one WAV per scene through piper-tts into `demo/out/audio/`.
5. Merges video + scene WAVs with ffmpeg into `docs/demo-video.mp4`.

Re-takes:

```powershell
# Skip the docker reset / seed when the stack is already populated
.\scripts\record-demo.ps1 -SkipSeed

# Just walk the scenario, don't narrate or merge (selector debugging)
.\scripts\record-demo.ps1 -RecordOnly
```

## Files

```
demo/
├── playwright.config.ts        Playwright run config (1 worker, video on)
├── scenarios/demo.spec.ts      The 12-scene scenario
├── lib/
│   ├── scenes.ts               Scene list (id, narration, min-duration)
│   ├── mailhog.ts              Reads + base64-decodes the welcome email
│   ├── narrate.ts              Renders one WAV per scene with piper-tts
│   └── merge.ts                ffmpeg audio + video merge
└── models/                     Piper voice (git-ignored; auto-downloaded)
```

## Tweaking the script

- **Narration**: edit the `narration` field on each scene in
  `lib/scenes.ts`. The piper output goes through `--length-scale 1.05` so the
  pace is calm — bump it up if you want it more brisk.
- **Pacing**: `minDurationMs` on each scene is the floor — the recorder
  always waits at least that long even when the UI catches up faster, so
  the audio has room.
- **Keystroke speed**: `KEYSTROKE_DELAY_MS` at the bottom of
  `scenarios/demo.spec.ts` controls how fast credentials and the card
  number are typed (the viewer needs to be able to read them).
