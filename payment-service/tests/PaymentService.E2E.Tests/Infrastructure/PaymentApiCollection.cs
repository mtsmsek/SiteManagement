namespace PaymentService.E2E.Tests.Infrastructure;

/// <summary>
/// Groups the PaymentService E2E tests so they share one <see cref="MongoFixture"/>
/// (a single Mongo container) across the whole class set, keeping the run cheap.
/// </summary>
[CollectionDefinition(Name)]
public sealed class PaymentApiCollection : ICollectionFixture<MongoFixture>
{
    /// <summary>The xunit collection name shared by every PaymentService E2E test.</summary>
    public const string Name = "PaymentService E2E";
}
