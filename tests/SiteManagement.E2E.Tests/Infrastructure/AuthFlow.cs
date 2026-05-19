using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// Helper that logs the bootstrap admin in and pre-configures an
/// <see cref="HttpClient"/> with the resulting access token. Used by every
/// admin-facing integration test so login boilerplate stays in one place.
/// </summary>
public static class AuthFlow
{
    private const string LoginPath = "/api/auth/login";
    private const string BearerScheme = "Bearer";

    /// <summary>Authenticates the bootstrap admin and returns the access token.</summary>
    public static async Task<string> LoginAsBootstrapAdminAsync(
        HttpClient client,
        CancellationToken ct = default)
    {
        var response = await client.PostAsJsonAsync(
            LoginPath,
            new
            {
                email = CustomWebApplicationFactory.BootstrapAdminEmail,
                password = CustomWebApplicationFactory.BootstrapAdminPassword,
            },
            ct);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginResponsePayload>(cancellationToken: ct);
        return payload!.AccessToken;
    }

    /// <summary>Replaces the client's default <c>Authorization</c> header with a Bearer token.</summary>
    public static void UseBearerToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerScheme, accessToken);
    }

    /// <summary>JSON serializer options matching the API's web defaults (camelCase).</summary>
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private sealed record LoginResponsePayload(string AccessToken);
}
