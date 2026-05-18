using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.ValueObjects;

namespace SiteManagement.Domain.Tests.Doubles;

/// <summary>
/// Shared factory helpers for Property-context tests. Centralising the
/// happy-path constructors keeps each test focused on the single rule it
/// exercises and stops sample literals from leaking across files.
/// </summary>
public static class PropertyDoubles
{
    /// <summary>Default valid apartment type used across tests.</summary>
    public const string SampleApartmentTypeRaw = "2+1";

    /// <summary>Convenience: a valid <see cref="ApartmentType"/> for use in arrange blocks.</summary>
    public static ApartmentType SampleApartmentType() => ApartmentType.From(SampleApartmentTypeRaw);

    /// <summary>Convenience: a valid <see cref="ApartmentNumber"/> with the supplied or default value.</summary>
    public static ApartmentNumber SampleApartmentNumber(int value = 1) => ApartmentNumber.From(value);

    /// <summary>Convenience: a valid <see cref="Floor"/> with the supplied or default value.</summary>
    public static Floor SampleFloor(int value = 1) => Floor.From(value);

    /// <summary>Convenience: a valid <see cref="BlockName"/> with the supplied or default value.</summary>
    public static BlockName SampleBlockName(string value = "A") => BlockName.From(value);

    /// <summary>Convenience: a fully-formed apartment created via the canonical factory.</summary>
    public static Apartment SampleApartment(int number = 1, int floor = 1, string type = SampleApartmentTypeRaw)
        => Apartment.Create(
            ApartmentNumber.From(number),
            Floor.From(floor),
            ApartmentType.From(type));
}
