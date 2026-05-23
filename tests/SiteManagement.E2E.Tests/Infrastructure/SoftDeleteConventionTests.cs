using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Shared;
using SiteManagement.Infrastructure.Persistence;

namespace SiteManagement.E2E.Tests.Infrastructure;

/// <summary>
/// Guards the soft-delete convention: any entity that opts into
/// <see cref="ISoftDeletable"/> must also declare a global query filter, or
/// archived rows would silently leak into reads. Builds the model only (no
/// database connection), so it runs fast and outside the Postgres collection.
/// </summary>
public class SoftDeleteConventionTests
{
    [Fact]
    public void Every_SoftDeletable_EntityType_HasAQueryFilter()
    {
        // arrange — build the EF model without touching a database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Database=model-only")
            .Options;
        using var dbContext = new AppDbContext(options);

        // act — soft-deletable entity types that forgot their query filter
        var offenders = dbContext.Model.GetEntityTypes()
            .Where(t => typeof(ISoftDeletable).IsAssignableFrom(t.ClrType))
            .Where(t => t.GetQueryFilter() is null)
            .Select(t => t.ClrType.Name)
            .ToArray();

        // assert
        offenders.Should().BeEmpty(
            "every ISoftDeletable entity needs a query filter or archived rows leak into reads (offenders: {0})",
            string.Join(", ", offenders));
    }
}
