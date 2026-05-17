using FluentAssertions;
using NetArchTest.Rules;

namespace SiteManagement.ArchitectureTests;

/// <summary>
/// Locks the Clean Architecture dependency direction in place. The build
/// would also fail if Domain referenced Application directly, but these
/// tests catch the subtler "Application accidentally pulled in EF Core via
/// a using" mistakes that a project reference graph can't see.
/// </summary>
public class LayerDependencyTests
{
    private const string EntityFrameworkNamespace = "Microsoft.EntityFrameworkCore";
    private const string AspNetCoreNamespace = "Microsoft.AspNetCore";
    private const string IdentityNamespace = "Microsoft.AspNetCore.Identity";

    /// <summary>Domain must depend only on the BCL — no Application, Infrastructure, Api, EF Core, or ASP.NET Core.</summary>
    [Fact]
    public void Domain_DoesNotDependOnAnyOtherLayerOrFramework()
    {
        // arrange
        var forbiddenNamespaces = new[]
        {
            "SiteManagement.Application",
            "SiteManagement.Infrastructure",
            "SiteManagement.Api",
            EntityFrameworkNamespace,
            AspNetCoreNamespace,
            "MediatR",
            "FluentValidation",
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
            .HaveDependencyOnAny(["SiteManagement.Infrastructure", "SiteManagement.Api"])
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Application must depend on Domain only. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    /// <summary>Application must not depend on EF Core — persistence is an Infrastructure concern.</summary>
    [Fact]
    public void Application_DoesNotDependOnEntityFrameworkCore()
    {
        // act
        var result = Types
            .InAssembly(AssemblyReferences.Application)
            .ShouldNot()
            .HaveDependencyOn(EntityFrameworkNamespace)
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Application must not pull in EF Core. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    /// <summary>Application must not depend on ASP.NET Core hosting / MVC / Identity.</summary>
    [Fact]
    public void Application_DoesNotDependOnAspNetCore()
    {
        // arrange — Localization.Abstractions lives under Microsoft.Extensions, not AspNetCore, so it's fine.
        // act
        var result = Types
            .InAssembly(AssemblyReferences.Application)
            .ShouldNot()
            .HaveDependencyOnAny([AspNetCoreNamespace, IdentityNamespace])
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Application must stay free of ASP.NET Core types. Failing types: {0}",
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
            .HaveDependencyOn("SiteManagement.Api")
            .GetResult();

        // assert
        result.IsSuccessful.Should().BeTrue(
            "Infrastructure must not depend on Api. Failing types: {0}",
            FormatFailing(result.FailingTypeNames));
    }

    private static string FormatFailing(IEnumerable<string>? failing)
        => failing is null ? "(none)" : string.Join(", ", failing);
}
