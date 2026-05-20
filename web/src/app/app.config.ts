import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideTranslateService, provideTranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader, provideTranslateHttpLoader } from '@ngx-translate/http-loader';

import { routes } from './app.routes';
import { tokenInterceptor } from './core/auth/token.interceptor';
import { errorInterceptor } from './core/http/error.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),

    // Angular Material still relies on the animations provider. The async
    // variant carries a deprecation hint (the framework is migrating to the
    // template-level animate.enter/leave API) but remains Material's
    // recommended wiring through v21; revisit in W6 when Material catches up.
    provideAnimationsAsync(),

    // Interceptor order: token (auth header + 401 refresh) runs first, then
    // error (snackbar for non-validation failures).
    provideHttpClient(withInterceptors([tokenInterceptor, errorInterceptor])),

    provideTranslateService({
      fallbackLang: 'tr',
      lang: 'tr',
      loader: provideTranslateLoader(TranslateHttpLoader),
    }),
    provideTranslateHttpLoader({ prefix: '/i18n/', suffix: '.json' }),
  ],
};
