import { writeFileSync } from 'node:fs';

/** A single cue: when on the timeline, for how long, with what text. */
export interface SrtCue {
  startMs: number;
  endMs: number;
  text: string;
}

/**
 * Writes a SubRip (SRT) file. Format is intentionally minimal — one cue per
 * scene, no line wrapping (ffmpeg's <c>subtitles</c> filter and every modern
 * player handle long lines fine).
 */
export function writeSrt(path: string, cues: SrtCue[]): void {
  const blocks: string[] = [];
  cues.forEach((cue, index) => {
    blocks.push(
      `${index + 1}\n${formatTimestamp(cue.startMs)} --> ${formatTimestamp(cue.endMs)}\n${cue.text}\n`,
    );
  });
  writeFileSync(path, blocks.join('\n'), 'utf-8');
}

/** SRT timestamps look like "00:00:12,345" — comma separates the millis. */
function formatTimestamp(ms: number): string {
  const hours = Math.floor(ms / 3_600_000);
  const minutes = Math.floor((ms % 3_600_000) / 60_000);
  const seconds = Math.floor((ms % 60_000) / 1_000);
  const millis = ms % 1_000;
  return `${two(hours)}:${two(minutes)}:${two(seconds)},${three(millis)}`;
}

const two = (n: number) => n.toString().padStart(2, '0');
const three = (n: number) => n.toString().padStart(3, '0');
