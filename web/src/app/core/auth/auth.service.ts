import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CurrentUser, LoginRequest, Roles, TokensResponse } from './auth.models';

const ACCESS_TOKEN_KEY = 'sm.accessToken';
const REFRESH_TOKEN_KEY = 'sm.refreshToken';

/**
 * Central authentication state, exposed as signals. Holds the JWT pair in
 * localStorage, decodes the access token into a {@link CurrentUser}, and
 * drives the auth/role guards + the token interceptor.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiBaseUrl;

  /** Current decoded user, or null when signed out. Reactive. */
  private readonly currentUserSignal = signal<CurrentUser | null>(this.decodeStoredUser());

  /** Read-only view of the current user signal. */
  readonly currentUser = this.currentUserSignal.asReadonly();

  /** True when an unexpired access token is present. */
  readonly isAuthenticated = computed(() => this.currentUserSignal() !== null);

  /** True when the signed-in user is an admin. */
  readonly isAdmin = computed(() => this.currentUserSignal()?.roles.includes(Roles.admin) ?? false);

  /** True when the signed-in user is a resident (drives the resident portal guard). */
  readonly isResident = computed(() => this.currentUserSignal()?.roles.includes(Roles.resident) ?? false);

  /** Role-based landing route: residents to their portal, everyone else to the admin area. */
  readonly homeUrl = computed(() => (this.isResident() && !this.isAdmin() ? '/resident' : '/admin'));

  /** The current access token, or null. Read by the token interceptor. */
  get accessToken(): string | null {
    return localStorage.getItem(ACCESS_TOKEN_KEY);
  }

  /** The current refresh token, or null. */
  get refreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  /** Authenticates and stores the resulting token pair. */
  async login(request: LoginRequest): Promise<void> {
    const tokens = await firstValueFrom(
      this.http.post<TokensResponse>(`${this.baseUrl}/api/auth/login`, request),
    );
    this.storeTokens(tokens);
  }

  /** Exchanges the stored refresh token for a fresh pair. Returns false on failure. */
  async tryRefresh(): Promise<boolean> {
    const refreshToken = this.refreshToken;
    if (!refreshToken) {
      return false;
    }

    try {
      const tokens = await firstValueFrom(
        this.http.post<TokensResponse>(`${this.baseUrl}/api/auth/refresh`, { refreshToken }),
      );
      this.storeTokens(tokens);
      return true;
    } catch {
      this.logout();
      return false;
    }
  }

  /** Clears stored tokens and resets the current-user signal. */
  logout(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    this.currentUserSignal.set(null);
  }

  private storeTokens(tokens: TokensResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, tokens.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, tokens.refreshToken);
    this.currentUserSignal.set(decodeJwt(tokens.accessToken));
  }

  private decodeStoredUser(): CurrentUser | null {
    const token = this.accessToken;
    if (!token) {
      return null;
    }

    const user = decodeJwt(token);
    // Drop an expired token so the app boots in a clean signed-out state.
    return user;
  }
}

/**
 * Minimal JWT payload decode (no signature check — the server is the
 * authority; the client only reads claims to drive the UI). Returns null
 * when the token is malformed or already expired.
 */
function decodeJwt(token: string): CurrentUser | null {
  try {
    const payloadJson = atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/'));
    const payload = JSON.parse(payloadJson) as Record<string, unknown>;

    const expSeconds = payload['exp'] as number | undefined;
    if (expSeconds && expSeconds * 1000 <= Date.now()) {
      return null;
    }

    const roleClaim = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
      ?? payload['role'];

    return {
      userId: (payload['sub'] as string) ?? '',
      email: (payload['email']
        ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
        ?? '') as string,
      roles: normaliseRoles(roleClaim),
      residentId: (payload['resident_id'] as string | undefined) ?? null,
    };
  } catch {
    return null;
  }
}

/** Role claim can be a single string or an array; normalise to string[]. */
function normaliseRoles(claim: unknown): string[] {
  if (Array.isArray(claim)) {
    return claim as string[];
  }
  return typeof claim === 'string' ? [claim] : [];
}
