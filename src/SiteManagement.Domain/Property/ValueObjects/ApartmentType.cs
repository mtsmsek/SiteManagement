using System.Text.RegularExpressions;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Property.ValueObjects;

/// <summary>
/// Turkish-real-estate "rooms+living rooms" notation for apartment layout
/// (e.g. <c>"2+1"</c>). Parsed once at construction; the parts are exposed
/// as plain integers so domain logic can reason about size without
/// re-parsing strings.
/// </summary>
public sealed partial class ApartmentType : ValueObject
{
    private const int MinimumRooms = 1;
    private const int MinimumLivingRooms = 0;

    [GeneratedRegex(@"^(?<rooms>\d+)\+(?<living>\d+)$", RegexOptions.CultureInvariant)]
    private static partial Regex FormatPattern();

    /// <summary>Number of bedrooms (the "N" in N+M). Always &gt;= 1.</summary>
    public int Rooms { get; }

    /// <summary>Number of living rooms (the "M" in N+M). Always &gt;= 0.</summary>
    public int LivingRooms { get; }

    private ApartmentType(int rooms, int livingRooms)
    {
        Rooms = rooms;
        LivingRooms = livingRooms;
    }

    /// <summary>
    /// Parses a raw <c>"N+M"</c> string into an apartment type.
    /// </summary>
    /// <exception cref="InvalidApartmentTypeException">
    /// Thrown when the string is null, empty, or does not match the N+M shape
    /// with N &gt;= 1 and M &gt;= 0.
    /// </exception>
    public static ApartmentType From(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidApartmentTypeException(raw ?? string.Empty);
        }

        var match = FormatPattern().Match(raw);
        if (!match.Success)
        {
            throw new InvalidApartmentTypeException(raw);
        }

        var rooms = int.Parse(match.Groups["rooms"].Value);
        var livingRooms = int.Parse(match.Groups["living"].Value);

        if (rooms < MinimumRooms || livingRooms < MinimumLivingRooms)
        {
            throw new InvalidApartmentTypeException(raw);
        }

        return new ApartmentType(rooms, livingRooms);
    }

    /// <inheritdoc />
    public override string ToString() => $"{Rooms}+{LivingRooms}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Rooms;
        yield return LivingRooms;
    }
}
