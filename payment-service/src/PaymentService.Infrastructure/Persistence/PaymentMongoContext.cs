using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace PaymentService.Infrastructure.Persistence;

/// <summary>
/// Thin wrapper over the MongoDB database handle. Exposes typed collection
/// accessors so repositories don't each re-resolve the database; collection
/// names live here as the single source of truth.
/// </summary>
public sealed class PaymentMongoContext
{
    private readonly IMongoDatabase _database;

    // MongoDB.Driver 3.x no longer assumes a default Guid representation and
    // throws on Unspecified. Our ids are plain Guids, so register the standard
    // (UUID) representation once, process-wide, before any client is built.
    static PaymentMongoContext()
        => BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

    /// <summary>Connects using the configured connection string + database name.</summary>
    public PaymentMongoContext(IOptions<MongoOptions> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        _database = client.GetDatabase(settings.Database);
    }

    /// <summary>Returns a typed handle to a collection by name.</summary>
    public IMongoCollection<TDocument> Collection<TDocument>(string name)
        => _database.GetCollection<TDocument>(name);

    /// <summary>Runs a lightweight ping so health checks can confirm connectivity.</summary>
    public Task PingAsync(CancellationToken ct = default)
        => _database.RunCommandAsync<MongoDB.Bson.BsonDocument>("{ ping: 1 }", cancellationToken: ct);
}
