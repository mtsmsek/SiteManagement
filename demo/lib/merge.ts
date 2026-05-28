import { spawnSync } from 'node:child_process';
import { existsSync, mkdirSync, readFileSync, readdirSync, statSync } from 'node:fs';
import { resolve } from 'node:path';
import ffmpegStaticImport from 'ffmpeg-static';
import { scenes } from './scenes.js';
import { chunkNarration, distributeDuration } from './split.js';
import { writeSrt, SrtCue } from './srt.js';

/**
 * Merges the Playwright-recorded WebM with the per-scene narration WAVs into
 * a single MP4 in <c>docs/demo-video.mp4</c>. The scene-manifest written by
 * the recorder maps each scene id to the millisecond it started inside the
 * video — that millisecond drives the audio's <c>adelay</c> so the
 * voice-over lands at the same instant as the on-screen action.
 *
 * Subtitles: each scene's narration is split into short, single-line chunks
 * via <c>lib/split.ts</c> and given a fair share of the scene's measured
 * audio length. They are written to <c>docs/demo-video.en.srt</c> AND
 * burned into the video via ffmpeg's <c>subtitles</c> filter, so the
 * captions flow with the voice-over instead of dropping a whole paragraph
 * on screen at once.
 */

const ffmpegPath = (ffmpegStaticImport as unknown as string) ?? '';
if (!ffmpegPath || !existsSync(ffmpegPath)) {
  throw new Error('ffmpeg-static binary path not resolved; reinstall demo dependencies.');
}

const repoRoot = resolve(import.meta.dirname, '..', '..');
const playwrightOut = resolve(repoRoot, 'demo', 'out', 'playwright');
const audioDir = resolve(repoRoot, 'demo', 'out', 'audio');
const docsDir = resolve(repoRoot, 'docs');
const outputPath = resolve(docsDir, 'demo-video.mp4');
const enSrtPath = resolve(docsDir, 'demo-video.en.srt');

mkdirSync(docsDir, { recursive: true });

const manifestPath = findFirst(playwrightOut, 'scene-manifest.json');
if (!manifestPath) {
  throw new Error(
    `scene-manifest.json missing under ${playwrightOut}; rerun "npm run record" first.`,
  );
}
const manifest = JSON.parse(readFileSync(manifestPath, 'utf-8')) as {
  recordStartedAt: number;
  scenes: Array<{ id: string; startMs: number; endMs: number }>;
};

const durationsPath = resolve(audioDir, 'durations.json');
const narrationDurations: Record<string, number> = existsSync(durationsPath)
  ? (JSON.parse(readFileSync(durationsPath, 'utf-8')) as Record<string, number>)
  : {};

const videoPath = findFirst(playwrightOut, 'video.webm');
if (!videoPath) {
  throw new Error(`Playwright video.webm not found under ${playwrightOut}.`);
}

// --- subtitle cues -----------------------------------------------------------
// For every scene, chunk the narration into single-line subtitles and offset
// each chunk inside the scene's measured audio length. Fall back to the
// scene's recorded duration if a narration WAV is missing (RecordOnly debug
// paths) so the SRT stays valid.
const enCues: SrtCue[] = [];
for (const scene of scenes) {
  const timing = manifest.scenes.find((t) => t.id === scene.id);
  if (!timing) {
    throw new Error(`Scene ${scene.id} missing from the manifest.`);
  }
  const totalMs = narrationDurations[scene.id] ?? timing.endMs - timing.startMs;
  const chunks = chunkNarration(scene.narration);
  const distributed = distributeDuration(chunks, totalMs);
  for (const c of distributed) {
    enCues.push({
      startMs: timing.startMs + c.startMs,
      endMs: timing.startMs + c.endMs,
      text: c.text,
    });
  }
}
writeSrt(enSrtPath, enCues);
console.log(`subtitles written: ${enSrtPath} (${enCues.length} cues)`);

// --- ffmpeg filter graph -----------------------------------------------------
// Video: burn the chunked SRT in at the bottom, white text + black outline
// so it stays readable against any background frame.
// Audio: each WAV is delayed to its scene start with adelay, then they all
// amix into one master track.
const audioInputs = scenes.flatMap((scene) => {
  const wav = resolve(audioDir, `${scene.id}.wav`);
  if (!existsSync(wav)) {
    throw new Error(`Narration WAV missing for ${scene.id}; run "npm run narrate" first.`);
  }
  return ['-i', wav];
});

const audioFilterParts: string[] = [];
const mixInputs: string[] = [];
scenes.forEach((scene, index) => {
  const timing = manifest.scenes.find((t) => t.id === scene.id)!;
  // input 0 is the video; audio inputs start at index 1.
  audioFilterParts.push(`[${index + 1}:a]adelay=${timing.startMs}|${timing.startMs}[a${index}]`);
  mixInputs.push(`[a${index}]`);
});
const audioMix = `${mixInputs.join('')}amix=inputs=${scenes.length}:normalize=0:dropout_transition=0[aout]`;

const videoFilter = `[0:v]subtitles='${escapeForFfmpeg(enSrtPath)}':force_style='Fontsize=20,Outline=1.5,Shadow=0.5,BorderStyle=1,MarginV=40,Alignment=2'[v]`;

const filterComplex = [videoFilter, ...audioFilterParts, audioMix].join(';');

const ffmpegArgs = [
  '-y',
  '-i',
  videoPath,
  ...audioInputs,
  '-filter_complex',
  filterComplex,
  '-map',
  '[v]',
  '-map',
  '[aout]',
  '-c:v',
  'libx264',
  '-preset',
  'fast',
  '-crf',
  '22',
  '-c:a',
  'aac',
  '-b:a',
  '160k',
  '-movflags',
  '+faststart',
  outputPath,
];

console.log(`merging -> ${outputPath}`);
const result = spawnSync(ffmpegPath, ffmpegArgs, { stdio: ['ignore', 'inherit', 'inherit'] });
if (result.status !== 0) {
  throw new Error(`ffmpeg exited with ${result.status}`);
}

console.log(`OK done: ${outputPath}`);

/**
 * ffmpeg's <c>subtitles</c> filter expects POSIX-style paths even on
 * Windows, and any colon inside the path has to be escaped because the
 * filter syntax already uses <c>:</c> as the option separator. Without
 * this a Windows path like <c>C:/Users/...</c> would be parsed as filter
 * option <c>subtitles="C"</c> + a leftover key <c>/Users/...</c>.
 */
function escapeForFfmpeg(path: string): string {
  return path.replace(/\\/g, '/').replace(/:/g, '\\:');
}

/** Walks the directory tree looking for the first file with the given name. */
function findFirst(dir: string, filename: string): string | null {
  if (!existsSync(dir)) return null;
  const stack: string[] = [dir];
  while (stack.length > 0) {
    const current = stack.pop()!;
    for (const entry of readdirSync(current)) {
      const full = resolve(current, entry);
      const stat = statSync(full);
      if (stat.isDirectory()) {
        stack.push(full);
      } else if (entry === filename) {
        return full;
      }
    }
  }
  return null;
}
