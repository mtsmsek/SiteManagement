using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency.ValueObjects;

/// <summary>
/// A vehicle owned by a resident: a required <see cref="Plate"/> and an
/// optional free-form note ("Red Renault Megane"). Notes are capped at
/// <see cref="ResidencyLimits.VehicleNoteMaxLength"/> characters.
/// </summary>
public sealed class VehicleInfo : ValueObject
{
    /// <summary>License plate; always present.</summary>
    public PlateNumber Plate { get; }

    /// <summary>Optional free-form note, or null when none provided.</summary>
    public string? Note { get; }

    private VehicleInfo(PlateNumber plate, string? note)
    {
        Plate = plate;
        Note = note;
    }

    /// <summary>Creates a vehicle info, validating the optional note's length.</summary>
    /// <exception cref="VehicleNoteTooLongException">Thrown when the note exceeds the allowed maximum.</exception>
    public static VehicleInfo Create(PlateNumber plate, string? note = null)
    {
        string? normalisedNote = null;
        if (!string.IsNullOrWhiteSpace(note))
        {
            var trimmed = note.Trim();
            if (trimmed.Length > ResidencyLimits.VehicleNoteMaxLength)
            {
                throw new VehicleNoteTooLongException();
            }
            normalisedNote = trimmed;
        }

        return new VehicleInfo(plate, normalisedNote);
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Plate;
        yield return Note;
    }
}
