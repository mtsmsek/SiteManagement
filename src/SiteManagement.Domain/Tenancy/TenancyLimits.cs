namespace SiteManagement.Domain.Tenancy;

/// <summary>
/// Size limits for the Tenancy bounded context. Persistence-facing constants
/// (column widths for enum-as-string mappings) live here next to the domain
/// types they describe, so the EF configuration references a named limit
/// rather than a magic number — mirroring <c>PropertyLimits</c>.
/// </summary>
public static class TenancyLimits
{
    /// <summary>Max stored length of <see cref="TenantType"/> as a string ("Tenant").</summary>
    public const int TenantTypeMaxLength = 20;
}
