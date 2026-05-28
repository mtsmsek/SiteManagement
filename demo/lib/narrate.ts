import { spawnSync } from 'node:child_process';
import { mkdirSync } from 'node:fs';
import { resolve } from 'node:path';
import { scenes } from './scenes.js';

/**
 * Renders one WAV file per scene under <c>demo/out/audio/</c>, using piper-tts
 * (open-source, offline) with the en_US-lessac-medium model that ships under
 * <c>demo/models/</c>. Each scene's narration is fed through the python
 * module so we don't depend on a piper CLI being on PATH.
 */

const repoRoot = resolve(import.meta.dirname, '..', '..');
const modelPath = resolve(repoRoot, 'demo', 'models', 'en_US-lessac-medium.onnx');
const outDir = resolve(repoRoot, 'demo', 'out', 'audio');

mkdirSync(outDir, { recursive: true });

const failures: string[] = [];
for (const scene of scenes) {
  const target = resolve(outDir, `${scene.id}.wav`);
  console.log(`narrating ${scene.id} → ${target}`);

  const result = spawnSync(
    'python',
    ['-m', 'piper', '-m', modelPath, '-f', target, '--length-scale', '1.05'],
    { input: scene.narration, encoding: 'utf-8' },
  );

  if (result.status !== 0) {
    failures.push(`${scene.id}: piper exited with ${result.status}\n${result.stderr}`);
  }
}

if (failures.length > 0) {
  console.error('Narration failures:\n' + failures.join('\n---\n'));
  process.exit(1);
}

console.log(`✓ rendered ${scenes.length} clips into ${outDir}`);
