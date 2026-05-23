namespace SiteManagement.Infrastructure.Events;

/// <summary>
/// Tuning for the outbox background processor. Bound from the <c>Outbox</c>
/// configuration section; defaults are sensible for production and can be set
/// very high in tests to keep the timer from racing an explicit run.
/// </summary>
public sealed class OutboxOptions
{
    /// <summary>Configuration section name bound at startup.</summary>
    public const string SectionName = "Outbox";

    /// <summary>Seconds between processing passes.</summary>
    public int PollSeconds { get; init; } = 10;

    /// <summary>Maximum messages delivered per pass.</summary>
    public int BatchSize { get; init; } = 50;
}
