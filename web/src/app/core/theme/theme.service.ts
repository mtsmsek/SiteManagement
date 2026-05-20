import { Injectable, signal, effect, computed } from '@angular/core';

/** The user-selectable color modes. `system` follows the OS preference. */
export type ThemeMode = 'light' | 'dark' | 'system';

const STORAGE_KEY = 'sm.theme';

/**
 * Owns the active color mode. Persists the choice to localStorage and reflects
 * it onto the document root as a `.theme-light` / `.theme-dark` class (or none
 * for `system`), which drives the `color-scheme` switch declared in styles.scss.
 */
@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly modeSignal = signal<ThemeMode>(this.readStoredMode());

  /** The current mode as chosen by the user (may be `system`). */
  readonly mode = this.modeSignal.asReadonly();

  /** True when the effective scheme is dark, resolving `system` against the OS. */
  readonly isDark = computed(() => {
    const mode = this.modeSignal();
    if (mode === 'system') {
      return globalThis.matchMedia?.('(prefers-color-scheme: dark)').matches ?? false;
    }
    return mode === 'dark';
  });

  constructor() {
    effect(() => this.applyMode(this.modeSignal()));
  }

  /** Sets the mode explicitly and persists it. */
  setMode(mode: ThemeMode): void {
    this.modeSignal.set(mode);
    localStorage.setItem(STORAGE_KEY, mode);
  }

  /** Flips between light and dark, collapsing `system` to an explicit choice. */
  toggle(): void {
    this.setMode(this.isDark() ? 'light' : 'dark');
  }

  private applyMode(mode: ThemeMode): void {
    const root = document.documentElement.classList;
    root.remove('theme-light', 'theme-dark');
    if (mode === 'light') {
      root.add('theme-light');
    } else if (mode === 'dark') {
      root.add('theme-dark');
    }
  }

  private readStoredMode(): ThemeMode {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'light' || stored === 'dark' || stored === 'system' ? stored : 'system';
  }
}
