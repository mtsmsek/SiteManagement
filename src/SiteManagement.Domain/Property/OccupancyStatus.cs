namespace SiteManagement.Domain.Property;

/// <summary>
/// Whether an apartment is currently occupied. Drives whether residency
/// assignment / dues distribution can target it.
/// </summary>
public enum OccupancyStatus
{
    /// <summary>No resident currently assigned.</summary>
    Empty = 0,

    /// <summary>A resident is currently assigned.</summary>
    Occupied = 1,
}
