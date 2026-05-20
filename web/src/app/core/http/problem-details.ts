/**
 * RFC 7807 ProblemDetails shape returned by the API's
 * GlobalExceptionMiddleware. `errors` carries per-field validation
 * messages (already localized server-side); `detail` carries the
 * human-readable, localized message for everything else.
 */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  traceId?: string;
  messageKey?: string;
  errors?: Record<string, string[]>;
}

/** Type guard: is this object shaped like a ProblemDetails payload? */
export function isProblemDetails(value: unknown): value is ProblemDetails {
  return typeof value === 'object'
    && value !== null
    && ('detail' in value || 'errors' in value || 'title' in value);
}
