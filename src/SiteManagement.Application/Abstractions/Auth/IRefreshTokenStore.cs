namespace SiteManagement.Application.Abstractions.Auth;

/// <summary>
/// Stores issued refresh tokens so the refresh endpoint can validate them and
/// rotate them on each use. The W1 implementation is in-memory; the EF-backed
/// store arrives in W2 along with the residency context.
/// </summary>
public interface IRefreshTokenStore
{
    /// <summary>Persists a freshly issued refresh token for the user.</summary>
    Task StoreAsync(Guid userId, string refreshToken, DateTime expiresAtUtc, CancellationToken ct);

    /// <summary>
    /// Validates the given refresh token. On success returns the owning user id and
    /// invalidates the token so it can't be reused.
    /// </summary>
    /// <returns><c>null</c> when the token is unknown, expired, or already consumed.</returns>
    Task<Guid?> ConsumeAsync(string refreshToken, CancellationToken ct);
}
