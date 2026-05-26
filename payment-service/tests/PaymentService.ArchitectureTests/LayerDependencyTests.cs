using FluentAssertions;
using NetArchTest.Rules;

namespace PaymentService.ArchitectureTests;

/// <summary>
/// Locks the PaymentService's Clean Architecture dependency direction in place,
/// mirroring the main API's guardrails. Until now the Domain's framework-purity
/// was only implicit (its <c>.csproj</c> happens to carry no package references);
/// these tests make a leak — e.g. a Mongo <c>using</c> creeping into Domain —
/// fail loudly in CI rather than passing silently.
/// </summary>
public class LayerDependencyTests
{
    private const string MongoNamespace = "MongoDB";
    private const string EntityFrameworkNamespace = "Microsoft.EntityFrameworkCore";
    private const string AspNetCoreNamespace = "Microsoft.AspNetCore";

    /// <summary>Domain must depend only on the BCL — no other layer, no Mongo/EF/ASP.NET Core.</summary>
    [Fact]
    public void Domain_DoesNotDependOnAnyOtherLayerOrFramework()
    {
        // arrange
        var forbiddenNamespaces = new[]
        {
            "PaymentService.Application",
            "PaymentService.Infrastructure",
            "PaymentService.Api",
            MongoNamespace,
            EntityFrameworkNamespace,
            AspNetCoreNamespace,
        };

        // act
        var result = Types
            .InAssembly(AssemblyReferences.Domain)
            .ShouldNot()
            .HaveDependencyOnAny(forbiddenNamespaces)
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Domain must be framework-free. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    /// <summary>Application must not reference Infrastructure or Api types.</summary>
    [Fact]
    public void Application_DoesNotDependOnInfrastructureOrApi()
    {
        // act
        var result = Types
            .InAssembly(AssemblyReferences.Application)
            .ShouldNot()
            .HaveDependencyOnAny(["PaymentService.Infrastructure", "PaymentService.Api"])
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Application must depend on Domain only. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    /// <summary>Application must stay free of persistence (Mongo) and HTTP (ASP.NET Core) concerns.</summary>
    [Fact]
    public void Application_DoesNotDependOnMongoOrAspNetCore()
    {
        // act
        var result = Types
            .InAssembly(AssemblyReferences.Application)
            .ShouldNot()
            .HaveDependencyOnAny([MongoNamespace, AspNetCoreNamespace])
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Application must not pull in Mongo or ASP.NET Core. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    /// <summary>Infrastructure must not reference Api types (no inward arrow from infra to host).</summary>
    [Fact]
    public void Infrastructure_DoesNotDependOnApi()
    {
        // act
        var result = Types
            .InAssembly(AssemblyReferences.Infrastructure)
            .ShouldNot()
            .HaveDependencyOn("PaymentService.Api")
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Infrastructure must not depend on Api. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    private static string FormatFailing(IEnumerable<string>? failing)
        => failing is null ? "(none)" : string.Join(", ", failing);
}
