import { openSync, readSync, closeSync, statSync } from 'node:fs';

/**
 * Measures a piper-tts WAV's length in milliseconds straight from the file
 * header — no extra binary, no ffprobe. Piper writes plain PCM with the
 * canonical 44-byte RIFF/WAVE header, so the sample rate + bit depth +
 * channel count out of the fmt chunk plus the file size are enough to get
 * an exact duration.
 */
export function getWavDurationMs(path: string): number {
  const fd = openSync(path, 'r');
  try {
    const header = Buffer.alloc(44);
    readSync(fd, header, 0, 44, 0);
    if (header.toString('ascii', 0, 4) !== 'RIFF' || header.toString('ascii', 8, 12) !== 'WAVE') {
      throw new Error(`${path} does not look like a RIFF/WAVE file.`);
    }
    const numChannels = header.readUInt16LE(22);
    const sampleRate = header.readUInt32LE(24);
    const bitsPerSample = header.readUInt16LE(34);
    const bytesPerSample = bitsPerSample / 8;
    const fileSize = statSync(path).size;
    const dataBytes = fileSize - 44;
    const seconds = dataBytes / (sampleRate * numChannels * bytesPerSample);
    return Math.round(seconds * 1000);
  } finally {
    closeSync(fd);
  }
}
