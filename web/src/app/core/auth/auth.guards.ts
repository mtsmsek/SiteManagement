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
 * Admin-only guard. A signed-in non-admin (e.g. a resident) is bounced to their
 * own home rather than the login page, so a role mismatch lands somewhere useful
 * instead of looping through sign-in.
 */
export const adminGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated() && auth.isAdmin()) {
    return true;
  }

  if (auth.isAuthenticated()) {
    return router.createUrlTree([auth.homeUrl()]);
  }

  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

/**
 * Resident-only guard for the self-service portal. An admin (or anyone without
 * the Resident role) is redirected to their own home rather than shown a
 * forbidden screen.
 */
export const residentGuard: CanActivateFn = (_route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated() && auth.isResident()) {
    return true;
  }

  if (auth.isAuthenticated()) {
    return router.createUrlTree([auth.homeUrl()]);
  }

  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};
