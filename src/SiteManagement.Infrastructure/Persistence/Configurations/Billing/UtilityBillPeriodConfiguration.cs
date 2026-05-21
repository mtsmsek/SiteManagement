using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Billing;
using SiteManagement.Infrastructure.Persistence.Converters;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Billing;

/// <summary>
/// EF mapping for the <see cref="UtilityBillPeriod"/> aggregate root. Mirrors
/// <see cref="DuesPeriodConfiguration"/>: the invoice total is a
/// Money-as-decimal column, the per-apartment shares become an owned-collection
/// child table, and the utility type is stored as a string.
/// </summary>
public sealed class UtilityBillPeriodConfiguration : IEntityTypeConfiguration<UtilityBillPeriod>
{
    public void Configure(EntityTypeBuilder<UtilityBillPeriod> builder)
    {
        builder.ToTable(SchemaConstants.Tables.UtilityBillPeriods);

        builder.HasKey(p => p.Id);

        builder.Property<uint>(SchemaConstants.ConcurrencyTokenColumn)
            .HasColumnName(SchemaConstants.ConcurrencyTokenColumn)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(p => p.SiteId).IsRequired();

        builder.Property(p => p.Month)
            .HasConversion(new BillingMonthConverter())
            .HasMaxLength(BillingLimits.BillingMonthLength)
            .IsRequired();

        builder.Property(p => p.UtilityType)
            .HasConversion<string>()
            .HasMaxLength(BillingLimits.EnumNameMaxLength)
            .IsRequired();

        builder.Property(p => p.TotalAmount)
            .HasConversion(new MoneyConverter())
            .HasColumnName(SchemaConstants.Columns.Amount)
            .HasPrecision(BillingLimits.MoneyPrecision, BillingLimits.MoneyScale)
            .IsRequired();

        builder.Property(p => p.IsClosed).IsRequired();

        builder.OwnsMany(p => p.Items, items =>
        {
            items.ToTable(SchemaConstants.Tables.UtilityBillItems);

            items.WithOwner().HasForeignKey(SchemaConstants.ForeignKeys.UtilityBillPeriodId);
            items.HasKey(i => i.Id);

            items.Property(i => i.ApartmentId).HasColumnName(SchemaConstants.Columns.ApartmentId).IsRequired();
            items.Property(i => i.ResidentId).HasColumnName(SchemaConstants.Columns.ResidentId).IsRequired();

            items.Property(i => i.Amount)
                .HasConversion(new MoneyConverter())
                .HasColumnName(SchemaConstants.Columns.Amount)
                .HasPrecision(BillingLimits.MoneyPrecision, BillingLimits.MoneyScale)
                .IsRequired();

            items.Property(i => i.Status)
                .HasConversion<string>()
                .HasMaxLength(BillingLimits.EnumNameMaxLength)
                .IsRequired();

            items.HasIndex(SchemaConstants.ForeignKeys.UtilityBillPeriodId, SchemaConstants.Columns.ApartmentId)
                .IsUnique();
        });

        builder.Navigation(p => p.Items)
            .HasField(SchemaConstants.BackingFields.UtilityBillPeriodItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.DomainEvents);
    }
}
