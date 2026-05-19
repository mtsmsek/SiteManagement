namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// xunit collection definition that ties every API integration test to a
/// single shared <see cref="PostgresFixture"/>. Tests opt in by decorating
/// the class with <c>[Collection(ApiCollection.Name)]</c>.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<PostgresFixture>
{
    /// <summary>Name referenced by <c>[Collection(...)]</c> attributes on test classes.</summary>
    public const string Name = "api-integration";
}
