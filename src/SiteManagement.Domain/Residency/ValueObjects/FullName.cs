using SiteManagement.Domain.Residency.Exceptions;
using SiteManagement.Domain.Shared;

namespace SiteManagement.Domain.Residency.ValueObjects;

/// <summary>
/// Display name composed of a required first and last name. Each component
/// is trimmed and capped at <see cref="ResidencyLimits.NameComponentMaxLength"/>.
/// </summary>
public sealed class FullName : ValueObject
{
    /// <summary>Trimmed first name.</summary>
    public string FirstName { get; }

    /// <summary>Trimmed last name.</summary>
    public string LastName { get; }

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    /// <summary>Creates a full name from raw inputs.</summary>
    /// <exception cref="InvalidFullNameException">
    /// Thrown when either part is empty/whitespace or longer than the allowed maximum.
    /// </exception>
    public static FullName Create(string firstName, string lastName)
    {
        var first = Normalise(firstName);
        var last = Normalise(lastName);
        return new FullName(first, last);
    }

    /// <inheritdoc />
    public override string ToString() => $"{FirstName} {LastName}";

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    private static string Normalise(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidFullNameException();
        }

        var trimmed = raw.Trim();
        if (trimmed.Length > ResidencyLimits.NameComponentMaxLength)
        {
            throw new InvalidFullNameException();
        }

        return trimmed;
    }
}
