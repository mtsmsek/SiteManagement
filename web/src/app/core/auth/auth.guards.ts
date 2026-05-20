import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

/**
 * Blocks unauthenticated access; redirects to /login keeping the attempted
 * URL so the user lands back where they wanted after signing in.
 */
export const authGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

/**
 * Admin-only guard. A signed-in resident (resident_id claim present, no
 * Admin role) is sent to the login page rather than a forbidden screen —
 * the resident portal lands in a later week.
 */
export const adminGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated() && auth.isAdmin()) {
    return true;
  }

  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
