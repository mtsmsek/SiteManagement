using FluentAssertions;
using MediatR;
using SiteManagement.Application.Abstractions.Messaging;

namespace SiteManagement.ArchitectureTests;

/// <summary>
/// Guards the authorization convention: every MediatR request must declare
/// <em>exactly one</em> authorization requirement
/// (<see cref="IAdminRequest"/> / <see cref="IResidentRequest"/> /
/// <see cref="IPublicRequest"/>). <c>AuthorizationBehavior</c> enforces these at
/// runtime and fails closed; this test turns a forgotten (or ambiguous, double)
/// requirement into a build error too, so no endpoint can ship without a
/// conscious authorization decision.
/// </summary>
public class AuthorizationConventionsTests
{
    private static readonly Type[] RequestTypes = AssemblyReferences.Application
        .GetTypes()
        .Where(IsRequest)
        .ToArray();

    /// <summary>Each request implements one — and only one — authorization marker.</summary>
    [Fact]
    public void Every_Request_DeclaresExactlyOneAuthorizationRequirement()
    {
        // arrange
        var markers = new[] { typeof(IAdminRequest), typeof(IResidentRequest), typeof(IPublicRequest) };

        // act
        var offenders = RequestTypes
            .Select(t => new { Type = t, Count = markers.Count(m => m.IsAssignableFrom(t)) })
            .Where(x => x.Count != 1)
            .Select(x => $"{x.Type.FullName} ({x.Count} markers)")
            .ToArray();

        // assert
        offenders.Should().BeEmpty(
            "every request must implement exactly one of IAdminRequest/IResidentRequest/IPublicRequest (offenders: {0})",
            string.Join(", ", offenders));
    }

    private static bool IsRequest(Type type)
    {
        if (type is { IsAbstract: true } or { IsInterface: true })
        {
            return false;
        }

        return type.GetInterfaces().Any(i =>
            i == typeof(IRequest)
            || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>)));
    }
}
