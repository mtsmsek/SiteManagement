import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { catchError, throwError } from 'rxjs';
import { isProblemDetails } from './problem-details';

const SNACKBAR_DURATION_MS = 5000;

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
        snackBar.open(resolveMessage(error, translate), undefined, {
          duration: SNACKBAR_DURATION_MS,
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
