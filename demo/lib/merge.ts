import { spawnSync } from 'node:child_process';
import { existsSync, mkdirSync, readFileSync, readdirSync, statSync } from 'node:fs';
import { resolve } from 'node:path';
import ffmpegStaticImport from 'ffmpeg-static';
import { scenes } from './scenes.js';

/**
 * Merges the Playwright-recorded WebM with the per-scene narration WAVs into
 * a single MP4 in <c>docs/demo-video.mp4</c>. The scene-manifest written by
 * the recorder maps each scene id to the millisecond it started inside the
 * video — that millisecond drives the audio's <c>adelay</c> so the
 * voice-over lands at the same instant as the on-screen action.
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

const videoPath = findFirst(playwrightOut, 'video.webm');
if (!videoPath) {
  throw new Error(`Playwright video.webm not found under ${playwrightOut}.`);
}

const audioInputs = scenes.flatMap((scene) => {
  const wav = resolve(audioDir, `${scene.id}.wav`);
  if (!existsSync(wav)) {
    throw new Error(`Narration WAV missing for ${scene.id}; run "npm run narrate" first.`);
  }
  return ['-i', wav];
});

const filterParts: string[] = [];
const mixInputs: string[] = [];
scenes.forEach((scene, index) => {
  const timing = manifest.scenes.find((t) => t.id === scene.id);
  if (!timing) {
    throw new Error(`Scene ${scene.id} missing from the manifest.`);
  }
  // input 0 is the video, so audio inputs start at index 1.
  filterParts.push(`[${index + 1}:a]adelay=${timing.startMs}|${timing.startMs}[a${index}]`);
  mixInputs.push(`[a${index}]`);
});
const audioMix = `${mixInputs.join('')}amix=inputs=${scenes.length}:normalize=0:dropout_transition=0[aout]`;
const filterComplex = [...filterParts, audioMix].join(';');

const ffmpegArgs = [
  '-y',
  '-i',
  videoPath,
  ...audioInputs,
  '-filter_complex',
  filterComplex,
  '-map',
  '0:v',
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

console.log(`merging → ${outputPath}`);
const result = spawnSync(ffmpegPath, ffmpegArgs, { stdio: ['ignore', 'inherit', 'inherit'] });
if (result.status !== 0) {
  throw new Error(`ffmpeg exited with ${result.status}`);
}

console.log(`✓ done: ${outputPath}`);

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
