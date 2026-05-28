using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace SiteManagement.Api.Configuration;

/// <summary>
/// Configures the .NET built-in rate limiter with two named policies the
/// sensitive endpoints opt into: a per-IP fixed-window cap on login attempts
/// (defence against credential-stuffing) and a per-user sliding-window cap
/// on pay-by-card calls (defence against rapid retry / accidental loops).
/// Both reject excess traffic with HTTP 429 — the controller decorates the
/// endpoint with <see cref="EnableRateLimitingAttribute"/> by name; nothing
/// else needs to know.
/// </summary>
public static class RateLimitingExtensions
{
    /// <summary>Policy name for the login endpoint (IP-keyed fixed window).</summary>
    public const string LoginPolicy = "login-policy";

    /// <summary>Policy name for the pay-by-card endpoints (user-keyed sliding window).</summary>
    public const string PayByCardPolicy = "pay-by-card-policy";

    private const int LoginAttemptsPerMinute = 5;
    private const int PayCallsPerMinute = 10;

    /// <summary>Registers the rate limiter + the two named policies.</summary>
    public static IServiceCollection AddSiteManagementRateLimiter(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(LoginPolicy, ctx => RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = LoginAttemptsPerMinute,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true,
                }));

            options.AddPolicy(PayByCardPolicy, ctx => RateLimitPartition.GetSlidingWindowLimiter(
                partitionKey: ctx.User.Identity?.Name
                    ?? ctx.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",
                factory: _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = PayCallsPerMinute,
                    Window = TimeSpan.FromMinutes(1),
                    SegmentsPerWindow = 6,
                    QueueLimit = 0,
                    AutoReplenishment = true,
                }));
        });

        return services;
    }
}
