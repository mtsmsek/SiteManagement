namespace SiteManagement.Infrastructure.Persistence;

/// <summary>
/// Schema-level magic-value-free constants used by the EF Core
/// configurations: table names, shadow foreign-key column names, and the
/// private backing-field names that drive EF's read/write access for the
/// aggregate-internal collections.
/// </summary>
public static class SchemaConstants
{
    /// <summary>Table names — one per persisted root or owned collection.</summary>
    public static class Tables
    {
        public const string Sites = "Sites";
        public const string Blocks = "Blocks";
        public const string Apartments = "Apartments";
        public const string Residents = "Residents";
        public const string ResidentVehicles = "ResidentVehicles";

        // Tenancy
        public const string ApartmentAssignments = "ApartmentAssignments";

        // Billing — Approach A: dues and utility bills are separate aggregates.
        public const string DuesPeriods = "DuesPeriods";
        public const string DuesItems = "DuesItems";
        public const string UtilityBillPeriods = "UtilityBillPeriods";
        public const string UtilityBillItems = "UtilityBillItems";

        // Messaging — after-commit integration event delivery.
        public const string OutboxMessages = "OutboxMessages";

        // Identity tables — flattened from the default AspNet* prefixes.
        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string UserRoles = "UserRoles";
        public const string UserClaims = "UserClaims";
        public const string UserLogins = "UserLogins";
        public const string RoleClaims = "RoleClaims";
        public const string UserTokens = "UserTokens";
    }

    /// <summary>Shadow foreign-key column names created by EF's parent-child mapping.</summary>
    public static class ForeignKeys
    {
        public const string SiteId = "SiteId";
        public const string BlockId = "BlockId";
        public const string ResidentId = "ResidentId";
        public const string DuesPeriodId = "DuesPeriodId";
        public const string UtilityBillPeriodId = "UtilityBillPeriodId";
    }

    /// <summary>Owned/child entity column names mapped by the Billing configurations.</summary>
    public static class Columns
    {
        public const string ApartmentId = "ApartmentId";
        public const string ResidentId = "ResidentId";
        public const string Amount = "Amount";
    }

    /// <summary>
    /// Private backing-field names EF uses to read/write aggregate-internal
    /// collections without going through the read-only facade.
    /// </summary>
    public static class BackingFields
    {
        public const string SiteBlocks = "_blocks";
        public const string BlockApartments = "_apartments";
        public const string ResidentVehicles = "_vehicles";
        public const string DuesPeriodItems = "_items";
        public const string UtilityBillPeriodItems = "_items";
    }

    /// <summary>Shadow property name used as the synthetic PK on owned-collection rows.</summary>
    public const string OwnedSurrogateKey = "Id";

    /// <summary>
    /// Shadow concurrency-token property name. Mapped to Postgres' built-in
    /// <c>xmin</c> system column by the npgsql convention — no migration
    /// needed because xmin always exists. Used to stop EF from emitting
    /// empty UPDATEs when an aggregate's child collection changes but no
    /// scalar property does, and to give us real optimistic concurrency.
    /// </summary>
    public const string ConcurrencyTokenColumn = "xmin";
}
