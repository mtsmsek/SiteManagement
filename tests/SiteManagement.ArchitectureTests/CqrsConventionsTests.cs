using FluentAssertions;
using FluentValidation;
using MediatR;

namespace SiteManagement.ArchitectureTests;

/// <summary>
/// Enforces the CQRS-lite conventions used across the Application layer so
/// future contributors can't accidentally drift away from them.
/// </summary>
public class CqrsConventionsTests
{
    private const string CommandSuffix = "Command";
    private const string QuerySuffix = "Query";
    private const string HandlerSuffix = "Handler";
    private const string ValidatorSuffix = "Validator";

    private static readonly Type[] RequestTypes = AssemblyReferences.Application
        .GetTypes()
        .Where(IsRequest)
        .ToArray();

    /// <summary>Every <c>IRequest&lt;T&gt;</c> implementation must end with <c>Command</c> or <c>Query</c>.</summary>
    [Fact]
    public void Every_RequestType_NameEndsWithCommandOrQuery()
    {
        // arrange
        var offenders = RequestTypes
            .Where(t => !t.Name.EndsWith(CommandSuffix, StringComparison.Ordinal)
                     && !t.Name.EndsWith(QuerySuffix, StringComparison.Ordinal))
            .Select(t => t.FullName!)
            .ToArray();

        // assert
        offenders.Should().BeEmpty(
            "every MediatR request must be named <Something>Command or <Something>Query (offenders: {0})",
            string.Join(", ", offenders));
    }

    /// <summary>Every request must have at least one matching handler.</summary>
    [Fact]
    public void Every_RequestType_HasAtLeastOneHandler()
    {
        // arrange
        var handlerTypes = AssemblyReferences.Application
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType
                    && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                     || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>)))
                .Select(i => i.GetGenericArguments()[0]))
            .ToHashSet();

        var unhandled = RequestTypes
            .Where(req => !handlerTypes.Contains(req))
            .Select(req => req.FullName!)
            .ToArray();

        // assert
        unhandled.Should().BeEmpty(
            "every MediatR request must have a handler (orphan requests: {0})",
            string.Join(", ", unhandled));
    }

    /// <summary>Concrete handler classes should be <c>sealed</c> (no accidental inheritance).</summary>
    [Fact]
    public void Every_Handler_IsSealed()
    {
        // arrange
        var handlerClasses = AssemblyReferences.Application
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                     && t.Name.EndsWith(HandlerSuffix, StringComparison.Ordinal)
                     && t.GetInterfaces().Any(i => i.IsGenericType
                         && (i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)
                          || i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))))
            .ToArray();

        var unsealed = handlerClasses.Where(h => !h.IsSealed).Select(h => h.FullName!).ToArray();

        // assert
        unsealed.Should().BeEmpty(
            "MediatR handlers should be sealed (offenders: {0})",
            string.Join(", ", unsealed));
    }

    /// <summary>Commands that mutate state should be paired with a FluentValidation validator.</summary>
    [Fact]
    public void Every_Command_HasAValidator()
    {
        // arrange
        var validatedTypes = AssemblyReferences.Application
            .GetTypes()
            .Where(t => !t.IsAbstract && t.Name.EndsWith(ValidatorSuffix, StringComparison.Ordinal))
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                .Select(i => i.GetGenericArguments()[0]))
            .ToHashSet();

        var commands = RequestTypes
            .Where(t => t.Name.EndsWith(CommandSuffix, StringComparison.Ordinal))
            .ToArray();

        var commandsMissingValidator = commands
            .Where(c => !validatedTypes.Contains(c))
            .Select(c => c.FullName!)
            .ToArray();

        // assert
        commandsMissingValidator.Should().BeEmpty(
            "every Command should be paired with an AbstractValidator (missing: {0})",
            string.Join(", ", commandsMissingValidator));
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
