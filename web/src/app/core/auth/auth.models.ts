/**
 * Auth-related contracts shared across the core auth services. These mirror
 * the API's auth DTOs (POST /api/auth/login, /refresh) one-to-one.
 */

/** Request body for POST /api/auth/login. */
export interface LoginRequest {
  email: string;
  password: string;
}

/** Request body for POST /api/auth/refresh. */
export interface RefreshRequest {
  refreshToken: string;
}

/** Response body shared by login + refresh. */
export interface TokensResponse {
  accessToken: string;
  accessTokenExpiresAtUtc: string;
  refreshToken: string;
  refreshTokenExpiresAtUtc: string;
}

/** Decoded claims we care about from the access token. */
export interface CurrentUser {
  userId: string;
  email: string;
  roles: string[];
  residentId: string | null;
}

/** Stable role names — must match the API's Roles constants. */
export const Roles = {
  admin: 'Admin',
  resident: 'Resident',
} as const;
