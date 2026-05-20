using System.Globalization;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using SiteManagement.Application.Behaviors;
using SiteManagement.Application.Shared.Exceptions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Domain.Shared.Exceptions;
using AppException = SiteManagement.Application.Shared.Exceptions.ApplicationException;

namespace SiteManagement.Application.Tests.Behaviors;

/// <summary>
/// Verifies the contract at the Domain → Application boundary: a handler may
/// throw a <see cref="DomainException"/>, but everything past
/// <see cref="ExceptionTranslationBehavior{TRequest, TResponse}"/> sees only
/// <see cref="AppException"/> subclasses.
/// </summary>
public class ExceptionTranslationBehaviorTests
{
    private const string MessageKey = "Sample.Invariant";
    private const string LocalizedMessage = "A sample invariant was violated.";

    /// <summary>Domain exception is wrapped as <see cref="BusinessRuleViolationException"/> with the localized message.</summary>
    [Fact]
    public async Task Handle_WhenHandlerThrowsDomainException_TranslatesToBusinessRuleViolation()
    {
        // arrange
        var localizer = StubLocalizer(MessageKey, LocalizedMessage);
        var behavior = new ExceptionTranslationBehavior<SampleRequest, Unit>(localizer);
        RequestHandlerDelegate<Unit> next = (_) => throw new SampleDomainException(MessageKey);

        // act
        var act = () => behavior.Handle(new SampleRequest(), next, CancellationToken.None);

        // assert
        var ex = await act.Should().ThrowAsync<BusinessRuleViolationException>();
        ex.Which.Message.Should().Be(LocalizedMessage);
        ex.Which.MessageKey.Should().Be(MessageKey);
        ex.Which.InnerException.Should().BeOfType<SampleDomainException>();
    }

    /// <summary>Application exceptions bypass translation (they're already at the right layer).</summary>
    [Fact]
    public async Task Handle_WhenHandlerThrowsApplicationException_RethrowsAsIs()
    {
        // arrange
        var localizer = StubLocalizer(MessageKey, LocalizedMessage);
        var behavior = new ExceptionTranslationBehavior<SampleRequest, Unit>(localizer);
        var original = new EntityNotFoundException("Apartment", Guid.Empty);
        RequestHandlerDelegate<Unit> next = (_) => throw original;

        // act
        var act = () => behavior.Handle(new SampleRequest(), next, CancellationToken.None);

        // assert
        var thrown = await act.Should().ThrowAsync<EntityNotFoundException>();
        thrown.Which.Should().BeSameAs(original);
    }

    /// <summary>Success path returns the handler's result untouched.</summary>
    [Fact]
    public async Task Handle_WhenHandlerSucceeds_ReturnsResponseUnchanged()
    {
        // arrange
        var localizer = StubLocalizer(MessageKey, LocalizedMessage);
        var behavior = new ExceptionTranslationBehavior<SampleRequest, Unit>(localizer);
        RequestHandlerDelegate<Unit> next = (_) => Task.FromResult(Unit.Value);

        // act
        var result = await behavior.Handle(new SampleRequest(), next, CancellationToken.None);

        // assert
        result.Should().Be(Unit.Value);
    }

    /// <summary>
    /// Regression for the parametric-message bug: a domain exception that
    /// carries format arguments must come out with the placeholders filled in,
    /// not as the raw "'{0}' ..." template. Uses the real resx-backed localizer
    /// so it exercises the same lookup path as production.
    /// </summary>
    [Fact]
    public async Task Handle_WhenDomainExceptionHasArgs_FillsPlaceholdersInLocalizedMessage()
    {
        // arrange
        CultureInfo.CurrentUICulture = new CultureInfo("tr-TR");
        var localizer = RealLocalizer();
        var behavior = new ExceptionTranslationBehavior<SampleRequest, Unit>(localizer);
        RequestHandlerDelegate<Unit> next =
            (_) => throw new SampleDomainException("Property.Site.DuplicateBlockName", "A Blok");

        // act
        var act = () => behavior.Handle(new SampleRequest(), next, CancellationToken.None);

        // assert
        var ex = await act.Should().ThrowAsync<BusinessRuleViolationException>();
        ex.Which.Message.Should().Contain("A Blok");
        ex.Which.Message.Should().NotContain("{0}");
    }

    /// <summary>
    /// Builds the production resx-backed localizer directly (no DI container),
    /// so the test exercises the real ResourceManager lookup + string.Format
    /// path that production uses.
    /// </summary>
    private static IStringLocalizer<ErrorMessages> RealLocalizer()
    {
        var options = Options.Create(new LocalizationOptions());
        var factory = new ResourceManagerStringLocalizerFactory(options, NullLoggerFactory.Instance);
        return new StringLocalizer<ErrorMessages>(factory);
    }

    /// <summary>Builds an <see cref="IStringLocalizer{T}"/> that maps a single key to a value.</summary>
    private static IStringLocalizer<ErrorMessages> StubLocalizer(string key, string value)
    {
        var localizer = Substitute.For<IStringLocalizer<ErrorMessages>>();
        localizer[key].Returns(new LocalizedString(key, value, resourceNotFound: false));
        return localizer;
    }

    private sealed record SampleRequest : IRequest<Unit>;

    private sealed class SampleDomainException : DomainException
    {
        public SampleDomainException(string messageKey, params object[] args) : base(messageKey, args) { }
    }
}
