namespace SiteManagement.Api.Middleware;

/// <summary>
/// Stamps the response with a small, opinionated set of security headers
/// before the rest of the pipeline writes the body. Active only outside
/// Development so Scalar's inline HTML/JS UI is unaffected during local work.
/// HSTS is left to <c>UseHsts()</c> (added separately in the pipeline) so
/// browsers learn the upgrade-to-HTTPS hint with the canonical max-age.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    /// <summary>Adds the headers, then delegates to the next middleware.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "no-referrer";
        // CSP intentionally left off: the Angular SPA is served from a separate
        // origin, and Scalar in Development needs inline scripts. Configure
        // when the production host actually serves the bundle.

        await _next(context);
    }
}
