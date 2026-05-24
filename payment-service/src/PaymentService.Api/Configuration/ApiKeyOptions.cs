namespace PaymentService.Api.Configuration;

/// <summary>
/// Service-to-service API key settings. The main API sends this key on every
/// charge request; PaymentService rejects calls without it. A pragmatic
/// stand-in for mTLS / OAuth client-credentials, which would be the
/// production-grade choice but are out of scope for this showcase.
/// </summary>
public sealed class ApiKeyOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "ApiKey";

    /// <summary>Header the caller must send the key in.</summary>
    public const string HeaderName = "X-Api-Key";

    /// <summary>The expected key value.</summary>
    public string Value { get; init; } = string.Empty;
}
