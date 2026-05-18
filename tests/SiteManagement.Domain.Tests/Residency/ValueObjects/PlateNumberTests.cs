using FluentAssertions;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Residency.ValueObjects;

/// <summary>
/// Specifies the <see cref="PlateNumber"/> value object: Turkish format
/// (NN[A-Z]{1,3}NNNN with NN a valid 01..81 province code). Normalises
/// casing + spacing.
/// </summary>
public class PlateNumberTests
{
    [Theory]
    [InlineData("34ABC123", "34ABC123")]
    [InlineData("34abc123", "34ABC123")]
    [InlineData(" 34 abc 123 ", "34ABC123")]
    [InlineData("06A1234", "06A1234")]            // 1 letter + 4 digits
    [InlineData("81ABC12", "81ABC12")]            // highest valid province code (81), 3 letters + 2 digits
    [InlineData("01AB1234", "01AB1234")]          // lowest valid province code (01)
    public void From_ValidPlate_NormalisesAndExposesValue(string raw, string expected)
    {
        // act
        var plate = PlateNumber.From(raw);

        // assert
        plate.Value.Should().Be(expected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("00ABC123")]              // province 00 is invalid
    [InlineData("82ABC123")]              // province > 81 is invalid
    [InlineData("3ABC123")]               // single-digit province
    [InlineData("34123")]                 // no letter group
    [InlineData("34ABCDE123")]            // letter group too long (>3)
    [InlineData("34ABC1")]                // digit group too short
    [InlineData("34ABC12345")]            // digit group too long
    [InlineData("34-ABC-123")]            // forbidden hyphens
    public void From_InvalidPlate_Throws(string raw)
    {
        // act
        var act = () => PlateNumber.From(raw);

        // assert
        act.Should().Throw<InvalidPlateNumberException>();
    }

    [Fact]
    public void From_Null_Throws()
    {
        // act
        var act = () => PlateNumber.From(null!);

        // assert
        act.Should().Throw<InvalidPlateNumberException>();
    }

    [Fact]
    public void Equality_DifferentCasing_AreEqual()
    {
        // arrange
        var a = PlateNumber.From("34abc123");
        var b = PlateNumber.From("34ABC123");

        // assert
        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }
}
