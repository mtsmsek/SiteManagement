/**
 * Splits a scene's narration into short, single-line subtitle chunks and
 * distributes the scene's measured audio duration across them proportionally
 * to each chunk's character count. The goal is a subtitle track that flows
 * with the voice-over instead of one giant block per scene that covers half
 * the screen.
 */

const MAX_CHARS_PER_CHUNK = 60;

/** Breaks a long narration into chunks that fit on one subtitle line. */
export function chunkNarration(text: string, maxChars: number = MAX_CHARS_PER_CHUNK): string[] {
  const chunks: string[] = [];
  let remaining = text.trim().replace(/\s+/g, ' ');

  while (remaining.length > 0) {
    if (remaining.length <= maxChars) {
      chunks.push(remaining);
      break;
    }

    // Prefer to break on punctuation inside the window so a sentence end
    // doesn't get torn in half.
    let breakAt = -1;
    for (let i = maxChars; i > Math.floor(maxChars * 0.5); i--) {
      if (/[,.;:!?—–]/.test(remaining[i] ?? '')) {
        breakAt = i + 1;
        break;
      }
    }

    // No punctuation? Fall back to the last whitespace before the limit.
    if (breakAt === -1) {
      for (let i = maxChars; i > Math.floor(maxChars * 0.5); i--) {
        if (remaining[i] === ' ') {
          breakAt = i;
          break;
        }
      }
    }

    // Nothing reasonable in the window — hard break (very rare on English).
    if (breakAt === -1) {
      breakAt = maxChars;
    }

    chunks.push(remaining.substring(0, breakAt).trim());
    remaining = remaining.substring(breakAt).trim();
  }

  return chunks.filter((chunk) => chunk.length > 0);
}

/** One distributed chunk relative to the scene's start. */
export interface DistributedChunk {
  text: string;
  startMs: number;
  endMs: number;
}

/**
 * Spreads the scene's <c>totalMs</c> audio length across the chunks in
 * proportion to each chunk's character count, returning offsets relative to
 * the scene's start (0 = first cue starts with the scene).
 */
export function distributeDuration(chunks: string[], totalMs: number): DistributedChunk[] {
  if (chunks.length === 0) {
    return [];
  }

  const totalChars = chunks.reduce((sum, c) => sum + c.length, 0);
  const result: DistributedChunk[] = [];
  let cursor = 0;
  for (let i = 0; i < chunks.length; i++) {
    const chunk = chunks[i];
    const isLast = i === chunks.length - 1;
    const share = chunk.length / totalChars;
    const duration = isLast ? totalMs - cursor : Math.round(totalMs * share);
    result.push({ text: chunk, startMs: cursor, endMs: cursor + duration });
    cursor += duration;
  }
  return result;
}
