using System.Text.Json;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Infrastructure.Persistence.Outbox;

/// <summary>
/// A persisted integration event awaiting after-commit delivery. Written inside
/// the same transaction as the business change that raised it (so the two
/// commit atomically), then picked up by the outbox processor once the data is
/// durably committed. Not a domain entity — purely an infrastructure record.
/// </summary>
public sealed class OutboxMessage
{
    /// <summary>JSON options shared by serialize (write) and deserialize (process).</summary>
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Surrogate primary key.</summary>
    public Guid Id { get; private set; }

    /// <summary>The integration event's CLR type, resolved back on processing.</summary>
    public string Type { get; private set; } = default!;

    /// <summary>The event serialised as JSON.</summary>
    public string Content { get; private set; } = default!;

    /// <summary>When the event was raised (drives processing order).</summary>
    public DateTime OccurredOnUtc { get; private set; }

    /// <summary>When the event was successfully dispatched; null while pending.</summary>
    public DateTime? ProcessedOnUtc { get; private set; }

    /// <summary>Last delivery error, if any — left set while the row is retried.</summary>
    public string? Error { get; private set; }

    // EF Core materialisation ctor.
    private OutboxMessage()
    {
    }

    /// <summary>Captures an integration event for durable, after-commit delivery.</summary>
    public static OutboxMessage From(IIntegrationEvent integrationEvent)
    {
        var type = integrationEvent.GetType();
        return new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = type.FullName!,
            Content = JsonSerializer.Serialize(integrationEvent, type, SerializerOptions),
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
        };
    }

    /// <summary>Rehydrates the stored event using the supplied type resolver.</summary>
    public IDomainEvent Deserialize(Func<string, Type?> typeResolver)
    {
        var type = typeResolver(Type)
            ?? throw new InvalidOperationException($"Outbox event type '{Type}' could not be resolved.");

        return (IDomainEvent)JsonSerializer.Deserialize(Content, type, SerializerOptions)!;
    }

    /// <summary>Marks the message delivered and clears any prior error.</summary>
    public void MarkProcessed(DateTime processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
        Error = null;
    }

    /// <summary>Records a failed delivery attempt; the row stays pending for retry.</summary>
    public void RecordError(string error) => Error = error;
}
