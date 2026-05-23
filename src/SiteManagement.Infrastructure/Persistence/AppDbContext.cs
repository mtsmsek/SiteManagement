using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Billing;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Residency;
using SiteManagement.Domain.Tenancy;
using SiteManagement.Infrastructure.Identity;
using SiteManagement.Infrastructure.Persistence.Outbox;

namespace SiteManagement.Infrastructure.Persistence;

/// <summary>
/// EF Core context for the relational store. Inherits from
/// <see cref="IdentityDbContext{TUser, TRole, TKey}"/> so ASP.NET Core
/// Identity uses the same migrations as the rest of the domain; aggregate
/// roots register their mappings through <c>IEntityTypeConfiguration</c>
/// classes picked up by assembly scanning.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser, AppRole, Guid>(options)
{
    /// <summary>Property bounded context root set.</summary>
    public DbSet<Site> Sites => Set<Site>();

    /// <summary>Residency bounded context root set.</summary>
    public DbSet<Resident> Residents => Set<Resident>();

    /// <summary>Tenancy bounded context root set.</summary>
    public DbSet<ApartmentAssignment> ApartmentAssignments => Set<ApartmentAssignment>();

    /// <summary>Billing — dues period root set.</summary>
    public DbSet<DuesPeriod> DuesPeriods => Set<DuesPeriod>();

    /// <summary>Billing — utility bill period root set.</summary>
    public DbSet<UtilityBillPeriod> UtilityBillPeriods => Set<UtilityBillPeriod>();

    /// <summary>Outbox — integration events awaiting after-commit delivery.</summary>
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplyShorterIdentityTableNames(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>Flattens the default <c>AspNet*</c> prefixes so the schema reads like the rest of the domain.</summary>
    private static void ApplyShorterIdentityTableNames(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(b => b.ToTable(SchemaConstants.Tables.Users));
        modelBuilder.Entity<AppRole>(b => b.ToTable(SchemaConstants.Tables.Roles));
        modelBuilder.Entity<IdentityUserRole<Guid>>(b => b.ToTable(SchemaConstants.Tables.UserRoles));
        modelBuilder.Entity<IdentityUserClaim<Guid>>(b => b.ToTable(SchemaConstants.Tables.UserClaims));
        modelBuilder.Entity<IdentityUserLogin<Guid>>(b => b.ToTable(SchemaConstants.Tables.UserLogins));
        modelBuilder.Entity<IdentityRoleClaim<Guid>>(b => b.ToTable(SchemaConstants.Tables.RoleClaims));
        modelBuilder.Entity<IdentityUserToken<Guid>>(b => b.ToTable(SchemaConstants.Tables.UserTokens));
    }
}
