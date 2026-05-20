import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideAppInitializer,
  inject,
} from '@angular/core';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideTranslateService, provideTranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader, TRANSLATE_HTTP_LOADER_CONFIG } from '@ngx-translate/http-loader';

import { routes } from './app.routes';
import { tokenInterceptor } from './core/auth/token.interceptor';
import { errorInterceptor } from './core/http/error.interceptor';
import { LanguageService } from './core/i18n/language.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes, withComponentInputBinding()),

    // Angular Material still relies on the animations provider. The async
    // variant carries a deprecation hint (the framework is migrating to the
    // template-level animate.enter/leave API) but remains Material's
    // recommended wiring through v21; revisit in W6 when Material catches up.
    provideAnimationsAsync(),

    // Interceptor order: token (auth header + 401 refresh) runs first, then
    // error (snackbar for non-validation failures).
    provideHttpClient(withInterceptors([tokenInterceptor, errorInterceptor])),

    // ngx-translate's provideTranslateService always registers a loader into
    // its own `loader` slot (defaulting to TranslateNoOpLoader, which loads
    // nothing). So the HTTP loader MUST be passed via that slot, not as a
    // sibling top-level provider, or the NoOp loader wins and every key
    // renders raw. provideTranslateLoader binds TranslateLoader ->
    // TranslateHttpLoader; the loader reads its prefix/suffix from the
    // TRANSLATE_HTTP_LOADER_CONFIG token provided alongside it.
    { provide: TRANSLATE_HTTP_LOADER_CONFIG, useValue: { prefix: '/i18n/', suffix: '.json', enforceLoading: true } },
    provideTranslateService({
      fallbackLang: 'tr',
      lang: 'tr',
      loader: provideTranslateLoader(TranslateHttpLoader),
    }),

    // Load + activate the persisted language before first paint. LanguageService
    // works around this ngx-translate build, where use() emits a loaded bundle
    // but never persists it into the store, so the pipe falls back to raw keys.
    provideAppInitializer(() => inject(LanguageService).use(inject(LanguageService).current())),
  ],
};
