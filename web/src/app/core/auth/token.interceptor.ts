import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/**
 * Attaches the bearer token to every API request and, on a 401, attempts a
 * one-shot refresh before retrying. If refresh fails the user is bounced to
 * the login page. Auth endpoints themselves are skipped so a failed login
 * doesn't trigger a refresh loop.
 */
export const tokenInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const isAuthEndpoint = req.url.includes('/api/auth/');
  const token = auth.accessToken;

  const authorisedReq = token && !isAuthEndpoint
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authorisedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401 || isAuthEndpoint) {
        return throwError(() => error);
      }

      // Try to refresh once, then replay the original request.
      return from(auth.tryRefresh()).pipe(
        switchMap((refreshed) => {
          if (!refreshed) {
            void router.navigate(['/login']);
            return throwError(() => error);
          }

          const retried = req.clone({
            setHeaders: { Authorization: `Bearer ${auth.accessToken}` },
          });
          return next(retried);
        }),
      );
    }),
  );
};
