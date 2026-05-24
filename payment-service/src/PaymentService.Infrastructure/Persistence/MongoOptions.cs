namespace PaymentService.Infrastructure.Persistence;

/// <summary>
/// Binds the MongoDB connection settings from configuration
/// (<c>Mongo:ConnectionString</c> / <c>Mongo:Database</c>).
/// </summary>
public sealed class MongoOptions
{
    /// <summary>Configuration section name.</summary>
    public const string SectionName = "Mongo";

    /// <summary>MongoDB connection string (host, credentials, options).</summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>Database name the payment collections live in.</summary>
    public string Database { get; init; } = "payments";
}
