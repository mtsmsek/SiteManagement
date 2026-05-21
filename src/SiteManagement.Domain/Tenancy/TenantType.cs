namespace SiteManagement.Domain.Tenancy;

/// <summary>
/// Whether an apartment assignment is for the owner (malik) or a renting
/// tenant (kiracı). Billing targets the active assignment regardless of type;
/// the distinction matters for reporting and, later, owner-only actions.
/// </summary>
public enum TenantType
{
    /// <summary>The apartment's owner (malik) lives in / holds the unit.</summary>
    Owner = 0,

    /// <summary>A renting tenant (kiracı) occupies the unit.</summary>
    Tenant = 1,
}
