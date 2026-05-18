using FluentAssertions;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Residency.ValueObjects;

namespace SiteManagement.Domain.Tests.Residency.ValueObjects;

/// <summary>
/// Specifies the <see cref="VehicleInfo"/> value object: a plate (required)
/// plus an optional free-form note capped at
/// <see cref="ResidencyLimits.VehicleNoteMaxLength"/>.
/// </summary>
public class VehicleInfoTests
{
    [Fact]
    public void Create_OnlyPlate_NoteIsNull()
    {
        // arrange
        var plate = PlateNumber.From("34ABC123");

        // act
        var info = VehicleInfo.Create(plate);

        // assert
        info.Plate.Should().Be(plate);
        info.Note.Should().BeNull();
    }

    [Fact]
    public void Create_WithNote_ExposesNote()
    {
        // arrange
        var plate = PlateNumber.From("34ABC123");

        // act
        var info = VehicleInfo.Create(plate, "Red Renault");

        // assert
        info.Note.Should().Be("Red Renault");
    }

    [Fact]
    public void Create_WhitespaceNote_StoredAsNull()
    {
        // arrange
        var plate = PlateNumber.From("34ABC123");

        // act
        var info = VehicleInfo.Create(plate, "   ");

        // assert
        info.Note.Should().BeNull();
    }

    [Fact]
    public void Create_TooLongNote_Throws()
    {
        // arrange
        var plate = PlateNumber.From("34ABC123");
        var tooLong = new string('a', ResidencyLimits.VehicleNoteMaxLength + 1);

        // act
        var act = () => VehicleInfo.Create(plate, tooLong);

        // assert
        act.Should().Throw<VehicleNoteTooLongException>();
    }
}
