namespace SiteManagement.Application.Property.Queries;

/// <summary>
/// Lightweight row for the admin "all sites" list page. Holds only the
/// columns the UI renders directly — no <c>Blocks</c> array, no full
/// aggregate hydration — so the database can return many rows cheaply.
/// </summary>
public sealed record SiteListItemDto(
    Guid Id,
    string Name,
    string Address,
    int BlockCount,
    int ApartmentCount);
