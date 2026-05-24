using FluentAssertions;
using PaymentService.Domain.Exceptions;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Tests.ValueObjects;

/// <summary>
/// Specifies the card value objects: <see cref="CardNumber"/> (16 digits +
/// Luhn check), <see cref="Cvv"/> (3-4 digits), and <see cref="ExpiryDate"/>
/// (month/year, expiry evaluated against a supplied reference date so the rule
/// stays deterministic).
/// </summary>
public class CardDetailsTests
{
    // A Luhn-valid test PAN (Visa test number).
    private const string ValidPan = "4242424242424242";

    [Fact]
    public void CardNumber_ValidLuhn_IsAccepted()
    {
        // act
        var card = CardNumber.From(ValidPan);

        // assert
        card.Value.Should().Be(ValidPan);
    }

    [Theory]
    [InlineData("4242424242424241")] // fails Luhn
    [InlineData("123")]              // too short
    [InlineData("42424242424242424")] // too long
    [InlineData("4242abcd42424242")] // non-digit
    [InlineData("")]
    public void CardNumber_Invalid_Throws(string raw)
    {
        // act
        var act = () => CardNumber.From(raw);

        // assert
        act.Should().Throw<InvalidCardException>();
    }

    [Fact]
    public void CardNumber_StripsSpaces()
    {
        // act — users often paste grouped digits
        var card = CardNumber.From("4242 4242 4242 4242");

        // assert
        card.Value.Should().Be(ValidPan);
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1234")]
    public void Cvv_ThreeOrFourDigits_IsAccepted(string raw)
    {
        // assert
        Cvv.From(raw).Value.Should().Be(raw);
    }

    [Theory]
    [InlineData("12")]
    [InlineData("12345")]
    [InlineData("12a")]
    [InlineData("")]
    public void Cvv_Invalid_Throws(string raw)
    {
        // act
        var act = () => Cvv.From(raw);

        // assert
        act.Should().Throw<InvalidCardException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void ExpiryDate_MonthOutOfRange_Throws(int month)
    {
        // act
        var act = () => ExpiryDate.Of(2030, month);

        // assert
        act.Should().Throw<InvalidCardException>();
    }

    [Fact]
    public void ExpiryDate_IsExpired_WhenReferenceIsAfterEndOfMonth()
    {
        // arrange — card valid through end of 2026-05
        var expiry = ExpiryDate.Of(2026, 5);

        // assert
        expiry.IsExpired(new DateOnly(2026, 5, 31)).Should().BeFalse(); // last valid day
        expiry.IsExpired(new DateOnly(2026, 6, 1)).Should().BeTrue();   // month rolled over
        expiry.IsExpired(new DateOnly(2026, 4, 15)).Should().BeFalse();
    }
}
