namespace SiteManagement.Application.Property.Queries;

/// <summary>
/// Site detail projection used by the admin detail page. Includes the
/// blocks and their apartments without exposing the underlying domain
/// types to the API layer.
/// </summary>
public sealed record SiteDetailsDto(
    Guid Id,
    string Name,
    string Address,
    IReadOnlyList<BlockSummaryDto> Blocks);

/// <summary>One block row inside a <see cref="SiteDetailsDto"/>.</summary>
public sealed record BlockSummaryDto(
    Guid Id,
    string Name,
    IReadOnlyList<ApartmentSummaryDto> Apartments);

/// <summary>One apartment row inside a <see cref="BlockSummaryDto"/>.</summary>
public sealed record ApartmentSummaryDto(
    Guid Id,
    int Number,
    int Floor,
    string Type,
    string Status);
