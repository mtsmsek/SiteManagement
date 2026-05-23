using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Domain.Shared;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.Infrastructure.Events;

/// <summary>
/// EF Core-backed <see cref="IOutboxProcessor"/>. Reads the oldest pending
/// messages, rehydrates each integration event and re-dispatches it through the
/// same <see cref="IDomainEventDispatcher"/> the in-transaction path uses, then
/// marks it delivered. A failed delivery records the error and leaves the row
/// pending, so the next pass retries it (at-least-once delivery).
/// </summary>
public sealed class OutboxProcessor(
    AppDbContext dbContext,
    IDomainEventDispatcher dispatcher,
    TimeProvider clock,
    IOptions<OutboxOptions> options)
    : IOutboxProcessor
{
    // Integration events live in the Domain assembly; resolve stored type names from there.
    private static readonly Assembly DomainAssembly = typeof(IDomainEvent).Assembly;

    private readonly AppDbContext _dbContext = dbContext;
    private readonly IDomainEventDispatcher _dispatcher = dispatcher;
    private readonly TimeProvider _clock = clock;
    private readonly OutboxOptions _options = options.Value;

    /// <inheritdoc />
    public async Task<int> ProcessPendingAsync(CancellationToken ct = default)
    {
        var pending = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        var delivered = 0;
        foreach (var message in pending)
        {
            try
            {
                var domainEvent = message.Deserialize(name => DomainAssembly.GetType(name));
                await _dispatcher.DispatchAsync([domainEvent], ct);
                message.MarkProcessed(_clock.GetUtcNow().UtcDateTime);
                delivered++;
            }
            catch (Exception ex)
            {
                // Leave the row pending; the next pass retries it.
                message.RecordError(ex.Message);
            }
        }

        await _dbContext.SaveChangesAsync(ct);
        return delivered;
    }
}
