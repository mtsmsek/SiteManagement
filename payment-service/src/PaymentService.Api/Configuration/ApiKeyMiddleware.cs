using Microsoft.Extensions.Options;

namespace PaymentService.Api.Configuration;

/// <summary>
/// Rejects requests to <c>/api/*</c> that don't carry the expected service
/// API key. Health and docs endpoints are left open. When no key is
/// configured (e.g. local dev / tests), the check is skipped so the service
/// stays usable without ceremony.
/// </summary>
public sealed class ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
{
    private readonly RequestDelegate _next = next;
    private readonly ApiKeyOptions _options = options.Value;

    /// <summary>Validates the API key header for protected paths.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var isProtected = context.Request.Path.StartsWithSegments("/api");
        var keyConfigured = !string.IsNullOrEmpty(_options.Value);

        if (isProtected && keyConfigured)
        {
            var provided = context.Request.Headers[ApiKeyOptions.HeaderName].ToString();
            if (!string.Equals(provided, _options.Value, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing or invalid API key.");
                return;
            }
        }

        await _next(context);
    }
}
