namespace SiteManagement.Domain.Property.Exceptions;

/// <summary>
/// Stable resource keys carried by Property-context domain exceptions. The
/// Application layer's <c>ExceptionTranslationBehavior</c> looks each key up
/// against the <c>ErrorMessages</c> resource bundle when translating a
/// <see cref="SiteManagement.Domain.Shared.Exceptions.DomainException"/> into
/// a localized <c>BusinessRuleViolationException</c>.
/// </summary>
public static class PropertyMessageKeys
{
    /// <summary><c>"Property.ApartmentType.Invalid"</c> — wrong format / out-of-range numbers.</summary>
    public const string ApartmentTypeInvalid = "Property.ApartmentType.Invalid";

    /// <summary><c>"Property.ApartmentNumber.OutOfRange"</c> — apartment number not within 1..999.</summary>
    public const string ApartmentNumberOutOfRange = "Property.ApartmentNumber.OutOfRange";

    /// <summary><c>"Property.Floor.OutOfRange"</c> — floor not within the supported range.</summary>
    public const string FloorOutOfRange = "Property.Floor.OutOfRange";

    /// <summary><c>"Property.BlockName.Invalid"</c> — empty or too long.</summary>
    public const string BlockNameInvalid = "Property.BlockName.Invalid";

    /// <summary><c>"Property.SiteName.Invalid"</c> — empty or too long.</summary>
    public const string SiteNameInvalid = "Property.SiteName.Invalid";

    /// <summary><c>"Property.Apartment.AlreadyOccupied"</c> — trying to occupy an already-occupied apartment.</summary>
    public const string ApartmentAlreadyOccupied = "Property.Apartment.AlreadyOccupied";

    /// <summary><c>"Property.Apartment.NotOccupied"</c> — trying to vacate an already-empty apartment.</summary>
    public const string ApartmentNotOccupied = "Property.Apartment.NotOccupied";

    /// <summary><c>"Property.Block.DuplicateApartmentNumber"</c> — apartment number already exists within the block.</summary>
    public const string DuplicateApartmentNumber = "Property.Block.DuplicateApartmentNumber";

    /// <summary><c>"Property.Block.ApartmentNotFound"</c> — apartment id not present in the block.</summary>
    public const string ApartmentNotFoundInBlock = "Property.Block.ApartmentNotFound";

    /// <summary><c>"Property.Site.DuplicateBlockName"</c> — two blocks with the same name within a site.</summary>
    public const string DuplicateBlockName = "Property.Site.DuplicateBlockName";

    /// <summary><c>"Property.Site.BlockNotFound"</c> — block id not present in the site.</summary>
    public const string BlockNotFoundInSite = "Property.Site.BlockNotFound";
}
