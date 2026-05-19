using System.Collections.Concurrent;
using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// First-pass <see cref="IRefreshTokenStore"/> implementation that keeps
/// issued refresh tokens in process memory. Suitable for local dev and the
/// W1 deploy; W2 swaps this for an EF-backed store keyed by user id so
/// tokens survive restarts and horizontal scaling.
/// </summary>
public class InMemoryRefreshTokenStore : IRefreshTokenStore
{
    private readonly ConcurrentDictionary<string, RefreshTokenEntry> _entries = new();

    /// <inheritdoc />
    public Task StoreAsync(Guid userId, string refreshToken, DateTime expiresAtUtc, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        _entries[refreshToken] = new RefreshTokenEntry(userId, expiresAtUtc);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<Guid?> ConsumeAsync(string refreshToken, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!_entries.TryRemove(refreshToken, out var entry))
        {
            return Task.FromResult<Guid?>(null);
        }

        if (entry.ExpiresAtUtc <= DateTime.UtcNow)
        {
            return Task.FromResult<Guid?>(null);
        }

        return Task.FromResult<Guid?>(entry.UserId);
    }

    /// <summary>
    /// Wipes every stored token. Used by integration tests to reset state
    /// between runs — production code never calls this.
    /// </summary>
    public void Clear() => _entries.Clear();

    /// <summary>Stored shape: owning user id and expiry timestamp (UTC).</summary>
    private sealed record RefreshTokenEntry(Guid UserId, DateTime ExpiresAtUtc);
}
