namespace SiteManagement.Infrastructure.Payments;

/// <summary>
/// Settings for reaching the external Payment service: its base URL and the
/// service-to-service API key. Bound from the <c>PaymentService</c>
/// configuration section.
/// </summary>
public sealed class PaymentServiceOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "PaymentService";

    /// <summary>Base URL of the Payment service (e.g. http://payment-api:8090).</summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>The API key sent on every charge request.</summary>
    public string ApiKey { get; init; } = string.Empty;
}
