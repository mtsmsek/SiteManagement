using FluentAssertions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Residency.ValueObjects;

/// <summary>
/// Specifies the <see cref="PhoneNumber"/> value object: accepts Turkish
/// mobile/landline numbers in three input forms and normalises them to a
/// canonical <c>+90XXXXXXXXXX</c> shape.
/// </summary>
public class PhoneNumberTests
{
    // Canonical form: +90 + 10 trunk digits. Mobile prefix is 5xx, but the
    // VO does not enforce mobile vs landline — that's a marketing concern,
    // not a domain invariant.
    private const string CanonicalMobile = "+905321234567";

    [Theory]
    [InlineData("+905321234567")]
    [InlineData("05321234567")]
    [InlineData("5321234567")]
    [InlineData("0532 123 45 67")]
    [InlineData("+90 (532) 123-45-67")]
    public void From_VariousInputs_NormaliseToCanonical(string raw)
    {
        // act
        var phone = PhoneNumber.From(raw);

        // assert
        phone.Value.Should().Be(CanonicalMobile);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("123")]                  // too short
    [InlineData("12345678901234567")]    // too long
    [InlineData("abcdefghij")]
    [InlineData("+15551234567")]         // not TR
    [InlineData("905321234567")]         // missing + and 0 prefix
    public void From_InvalidInput_Throws(string raw)
    {
        // act
        var act = () => PhoneNumber.From(raw);

        // assert
        act.Should().Throw<InvalidPhoneNumberException>();
    }

    [Fact]
    public void From_Null_Throws()
    {
        // act
        var act = () => PhoneNumber.From(null!);

        // assert
        act.Should().Throw<InvalidPhoneNumberException>();
    }

    [Fact]
    public void Equality_DifferentInputForms_AreEqual()
    {
        // arrange
        var a = PhoneNumber.From("05321234567");
        var b = PhoneNumber.From("+90 532 123 45 67");

        // assert
        a.Should().Be(b);
    }
}
