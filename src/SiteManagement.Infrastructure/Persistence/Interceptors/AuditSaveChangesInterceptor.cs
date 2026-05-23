using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SiteManagement.Application.Abstractions.Auth;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Stamps created/modified audit metadata on every <see cref="IAuditableEntity"/>
/// at save time. This is pure ambient metadata — it never alters business state
/// — so it belongs in a save interceptor rather than scattered across handlers.
/// The acting user comes from <see cref="ICurrentUser"/> (null for system or
/// background writes, which have no authenticated principal); the timestamp from
/// the injected <see cref="TimeProvider"/>. Values are written through the EF
/// property entry so the domain can keep private setters.
/// </summary>
public sealed class AuditSaveChangesInterceptor(ICurrentUser currentUser, TimeProvider clock)
    : SaveChangesInterceptor
{
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly TimeProvider _clock = clock;

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        var now = _clock.GetUtcNow().UtcDateTime;
        var userId = _currentUser.IsAuthenticated ? _currentUser.UserId : (Guid?)null;

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Property(nameof(IAuditableEntity.CreatedAtUtc)).CurrentValue = now;
                    entry.Property(nameof(IAuditableEntity.CreatedBy)).CurrentValue = userId;
                    break;

                case EntityState.Modified:
                    entry.Property(nameof(IAuditableEntity.ModifiedAtUtc)).CurrentValue = now;
                    entry.Property(nameof(IAuditableEntity.ModifiedBy)).CurrentValue = userId;
                    break;
            }
        }
    }
}
