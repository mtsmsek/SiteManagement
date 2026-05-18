using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Residency;
using SiteManagement.Infrastructure.Identity;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Identity;

/// <summary>
/// EF mapping additions on top of the defaults supplied by
/// <c>IdentityDbContext</c>. Adds the <see cref="AppUser.ResidentId"/>
/// foreign key column with a unique index (one user per resident) so the
/// database itself blocks accidental double-links.
/// </summary>
public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(ResidencyLimits.NameComponentMaxLength * 2 + 1);

        // ResidentId is a "soft" FK — there is no Resident navigation on
        // AppUser (we don't want the Identity entity dragging Domain types
        // into queries), and there is no AppUser navigation on Resident
        // (the Domain stays unaware of Identity). The FK constraint is
        // expressed via HasOne(...).WithMany(...) on the Resident side
        // implicitly through the column + index combination.
        builder.HasIndex(u => u.ResidentId)
            .IsUnique()
            .HasFilter("\"ResidentId\" IS NOT NULL");
    }
}
