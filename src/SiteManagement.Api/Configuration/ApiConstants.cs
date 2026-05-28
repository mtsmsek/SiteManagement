namespace SiteManagement.Api.Configuration;

/// <summary>
/// API-layer-specific magic-value-free constants: route prefixes, auth scheme
/// names, ProblemDetails extension keys. Anything an inline literal would
/// otherwise sprinkle into a controller/middleware goes here.
/// </summary>
public static class ApiConstants
{
    /// <summary>Common route prefix for every controller in the API.</summary>
    public const string RoutePrefix = "api";

    /// <summary>Common route prefix for every SignalR hub mounted by the API.</summary>
    public const string HubsPrefix = "/hubs";

    /// <summary>The messaging hub endpoint path (resident ↔ admin real-time push).</summary>
    public const string MessagingHubPath = $"{HubsPrefix}/messaging";

    /// <summary>Query-string key SignalR clients use to pass the JWT over WebSockets.</summary>
    public const string SignalRAccessTokenQueryKey = "access_token";

    /// <summary>Section name in <c>appsettings.json</c> for CORS origins (W2).</summary>
    public const string CorsAllowFrontendPolicy = "AllowFrontend";

    /// <summary>Bearer scheme display name shown in Scalar's "Authorize" panel.</summary>
    public const string BearerSchemeName = "Bearer";

    /// <summary>Health check endpoint path.</summary>
    public const string HealthEndpoint = "/health";

    /// <summary>Liveness-only health endpoint (no DB probe).</summary>
    public const string HealthLiveEndpoint = "/health/live";

    /// <summary>Extension key in ProblemDetails carrying per-field validation errors.</summary>
    public const string ProblemDetailsErrorsKey = "errors";

    /// <summary>Extension key in ProblemDetails carrying the stable resource key for clients.</summary>
    public const string ProblemDetailsMessageKey = "messageKey";

    /// <summary>Extension key in ProblemDetails carrying the trace identifier.</summary>
    public const string ProblemDetailsTraceIdKey = "traceId";
}
