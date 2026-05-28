import { spawnSync } from 'node:child_process';
import { mkdirSync, writeFileSync } from 'node:fs';
import { resolve } from 'node:path';
import { scenes } from './scenes.js';
import { getWavDurationMs } from './wav-duration.js';

/**
 * Renders one WAV file per scene under <c>demo/out/audio/</c> with piper-tts
 * (open-source, offline) and records each clip's measured duration in a
 * sidecar JSON. The recorder reads that JSON at run-time and bumps every
 * scene's on-screen hold to <c>max(originalMin, narrationDuration + buffer)</c>,
 * which stops scene N+1's voice-over from starting before scene N's has
 * finished. The merge step then drops each WAV in at the start of its scene
 * and nothing overlaps.
 */

const repoRoot = resolve(import.meta.dirname, '..', '..');
const modelPath = resolve(repoRoot, 'demo', 'models', 'en_US-lessac-medium.onnx');
const outDir = resolve(repoRoot, 'demo', 'out', 'audio');

mkdirSync(outDir, { recursive: true });

const durations: Record<string, number> = {};
const failures: string[] = [];

for (const scene of scenes) {
  const target = resolve(outDir, `${scene.id}.wav`);
  console.log(`narrating ${scene.id} -> ${target}`);

  const result = spawnSync(
    'python',
    ['-m', 'piper', '-m', modelPath, '-f', target, '--length-scale', '1.05'],
    { input: scene.narration, encoding: 'utf-8' },
  );

  if (result.status !== 0) {
    failures.push(`${scene.id}: piper exited with ${result.status}\n${result.stderr}`);
    continue;
  }

  durations[scene.id] = getWavDurationMs(target);
}

if (failures.length > 0) {
  console.error('Narration failures:\n' + failures.join('\n---\n'));
  process.exit(1);
}

writeFileSync(resolve(outDir, 'durations.json'), JSON.stringify(durations, null, 2), 'utf-8');

console.log(`OK rendered ${scenes.length} clips into ${outDir}`);
for (const scene of scenes) {
  console.log(`  ${scene.id}: ${durations[scene.id]} ms`);
}
