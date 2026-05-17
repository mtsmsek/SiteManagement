using FluentAssertions;
using SiteManagement.Domain.Shared.Exceptions;

namespace SiteManagement.Domain.Tests.Shared.Exceptions;

/// <summary>Sanity tests for the <see cref="DomainException"/> base class.</summary>
public class DomainExceptionTests
{
    private const string MessageKey = "Sample.Key";

    /// <summary>Ctor stores the message key and the supplied arguments verbatim.</summary>
    [Fact]
    public void Ctor_StoresMessageKeyAndArgs()
    {
        // arrange
        const string firstArg = "first";
        const int secondArg = 42;

        // act
        var ex = new SampleException(MessageKey, firstArg, secondArg);

        // assert
        ex.MessageKey.Should().Be(MessageKey);
        ex.MessageArgs.Should().Equal(firstArg, secondArg);
    }

    /// <summary>Default constructor parameters yield an empty arg list (never null).</summary>
    [Fact]
    public void Ctor_WithoutArgs_StoresEmptyArgs()
    {
        // act
        var ex = new SampleException(MessageKey);

        // assert
        ex.MessageArgs.Should().BeEmpty();
        ex.MessageKey.Should().Be(MessageKey);
    }

    /// <summary>Concrete subclass used only by these tests.</summary>
    private sealed class SampleException : DomainException
    {
        public SampleException(string messageKey, params object[] args) : base(messageKey, args) { }
    }
}
