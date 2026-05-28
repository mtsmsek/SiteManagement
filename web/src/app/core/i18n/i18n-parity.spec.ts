import { readFileSync } from 'node:fs';
import { resolve } from 'node:path';

/**
 * Guardrail: every translation key in tr.json must exist in en.json and vice
 * versa. A forgotten translation surfaces here at build time instead of as a
 * runtime "missing translation" warning the operator sees in the browser.
 */
describe('i18n parity', () => {
  // Vitest runs with the web/ folder as its working directory.
  const i18nDir = resolve(process.cwd(), 'public/i18n');
  const tr = loadJson(`${i18nDir}/tr.json`);
  const en = loadJson(`${i18nDir}/en.json`);

  it('tr and en cover the same keys', () => {
    // arrange
    const trKeys = collectKeys(tr).sort();
    const enKeys = collectKeys(en).sort();

    // act
    const onlyInTr = trKeys.filter((k) => !enKeys.includes(k));
    const onlyInEn = enKeys.filter((k) => !trKeys.includes(k));

    // assert
    expect(onlyInTr).toEqual([]);
    expect(onlyInEn).toEqual([]);
  });
});

function loadJson(path: string): Record<string, unknown> {
  return JSON.parse(readFileSync(path, 'utf-8')) as Record<string, unknown>;
}

function collectKeys(node: unknown, prefix = ''): string[] {
  if (typeof node !== 'object' || node === null) {
    return [prefix];
  }
  return Object.entries(node as Record<string, unknown>).flatMap(([key, value]) =>
    collectKeys(value, prefix ? `${prefix}.${key}` : key),
  );
}
