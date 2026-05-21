using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Tenancy;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Tenancy;

/// <summary>
/// EF mapping for the <see cref="ApartmentAssignment"/> aggregate root.
/// Apartment / resident are plain Guid columns (cross-aggregate references by
/// id). The <c>AssignmentPeriod</c> value object flattens into Start/End
/// columns. A filtered unique index on ApartmentId WHERE the assignment is
/// still active enforces "at most one active assignment per apartment" at the
/// database boundary, complementing the domain invariant.
/// </summary>
public sealed class ApartmentAssignmentConfiguration : IEntityTypeConfiguration<ApartmentAssignment>
{
    private const string StartDateColumn = "Period_StartDate";
    private const string EndDateColumn = "Period_EndDate";

    public void Configure(EntityTypeBuilder<ApartmentAssignment> builder)
    {
        builder.ToTable(SchemaConstants.Tables.ApartmentAssignments);

        builder.HasKey(a => a.Id);

        builder.Property<uint>(SchemaConstants.ConcurrencyTokenColumn)
            .HasColumnName(SchemaConstants.ConcurrencyTokenColumn)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(a => a.ApartmentId).IsRequired();
        builder.Property(a => a.ResidentId).IsRequired();

        builder.Property(a => a.TenantType)
            .HasConversion<string>()
            .HasMaxLength(TenancyLimits.TenantTypeMaxLength)
            .IsRequired();

        builder.ComplexProperty(a => a.Period, period =>
        {
            period.Property(p => p.StartDate).HasColumnName(StartDateColumn).IsRequired();
            period.Property(p => p.EndDate).HasColumnName(EndDateColumn);
        });

        // At most one active (open-ended) assignment per apartment.
        builder.HasIndex(a => a.ApartmentId)
            .IsUnique()
            .HasFilter($"\"{EndDateColumn}\" IS NULL");

        builder.Ignore(a => a.DomainEvents);
        builder.Ignore(a => a.IsActive);
    }
}
