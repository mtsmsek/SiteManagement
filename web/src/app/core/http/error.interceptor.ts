import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { catchError, throwError } from 'rxjs';
import { isProblemDetails } from './problem-details';
import { ErrorSnackbar, ErrorSnackbarData } from './error-snackbar';

// A failed action lingers a touch longer than the usual toast so the user has
// time to read what went wrong before it auto-hides.
const SNACKBAR_DURATION_MS = 8000;

/**
 * Surfaces API failures as snackbars. The API already localizes the
 * `detail` field, so we show it verbatim; validation 400s are left for the
 * forms to render inline (we don't snackbar every field error). Network
 * failures (status 0) fall back to a translated generic message.
 */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const snackBar = inject(MatSnackBar);
  const translate = inject(TranslateService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // 401 handling lives in the token interceptor; validation 400s render
      // inline on the form. Everything else gets a snackbar.
      const isValidation = error.status === 400;
      const isUnauthorized = error.status === 401;

      if (!isValidation && !isUnauthorized) {
        // A failed action needs to stand out: a dedicated component (icon +
        // error colour + entry animation, see `.error-snackbar` in styles.scss)
        // shown at the top so a decline or server error is impossible to miss.
        snackBar.openFromComponent<ErrorSnackbar, ErrorSnackbarData>(ErrorSnackbar, {
          data: { message: resolveMessage(error, translate) },
          duration: SNACKBAR_DURATION_MS,
          panelClass: 'error-snackbar',
          horizontalPosition: 'center',
          verticalPosition: 'top',
        });
      }

      return throwError(() => error);
    }),
  );
};

function resolveMessage(error: HttpErrorResponse, translate: TranslateService): string {
  if (error.status === 0) {
    return translate.instant('errors.network');
  }

  if (isProblemDetails(error.error) && error.error.detail) {
    return error.error.detail;
  }

  return translate.instant('errors.generic');
}
