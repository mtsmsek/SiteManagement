import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { TranslateService, TranslateLoader } from '@ngx-translate/core';

/** Languages the UI ships translations for. */
export const SUPPORTED_LANGUAGES = ['tr', 'en'] as const;
export type Language = (typeof SUPPORTED_LANGUAGES)[number];

const STORAGE_KEY = 'sm.lang';

/**
 * Owns the active UI language. Wraps TranslateService because, in this
 * ngx-translate build, use() loads a bundle but does not persist it into the
 * translation store, so the pipe falls back to raw keys. This service fetches
 * via the loader and writes it with setTranslation before switching, then
 * caches loaded languages so repeat switches are instant.
 */
@Injectable({ providedIn: 'root' })
export class LanguageService {
  private readonly translate = inject(TranslateService);
  private readonly loader = inject(TranslateLoader);
  private readonly loaded = new Set<string>();

  /** The currently active language. */
  readonly current = signal<Language>(this.readStoredLanguage());

  /** Loads (if needed) and activates a language, persisting the choice. */
  async use(lang: Language): Promise<void> {
    if (!this.loaded.has(lang)) {
      const translations = await firstValueFrom(this.loader.getTranslation(lang));
      this.translate.setTranslation(lang, translations);
      this.loaded.add(lang);
    }
    this.translate.use(lang);
    this.current.set(lang);
    localStorage.setItem(STORAGE_KEY, lang);
  }

  private readStoredLanguage(): Language {
    const stored = localStorage.getItem(STORAGE_KEY);
    return SUPPORTED_LANGUAGES.includes(stored as Language) ? (stored as Language) : 'tr';
  }
}
