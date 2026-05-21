using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Billing;
using SiteManagement.Infrastructure.Persistence.Converters;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Billing;

/// <summary>
/// EF mapping for the <see cref="DuesPeriod"/> aggregate root. The fixed
/// per-apartment amount is a Money-as-decimal column; the per-apartment dues
/// items become an owned-collection child table, each with its own Money
/// amount and a unique index keeping one item per apartment per period.
/// </summary>
public sealed class DuesPeriodConfiguration : IEntityTypeConfiguration<DuesPeriod>
{
    public void Configure(EntityTypeBuilder<DuesPeriod> builder)
    {
        builder.ToTable(SchemaConstants.Tables.DuesPeriods);

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

        builder.Property(p => p.PerApartmentAmount)
            .HasConversion(new MoneyConverter())
            .HasColumnName(SchemaConstants.Columns.Amount)
            .HasPrecision(BillingLimits.MoneyPrecision, BillingLimits.MoneyScale)
            .IsRequired();

        builder.Property(p => p.IsClosed).IsRequired();

        builder.OwnsMany(p => p.Items, items =>
        {
            items.ToTable(SchemaConstants.Tables.DuesItems);

            items.WithOwner().HasForeignKey(SchemaConstants.ForeignKeys.DuesPeriodId);
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

            items.HasIndex(SchemaConstants.ForeignKeys.DuesPeriodId, SchemaConstants.Columns.ApartmentId)
                .IsUnique();
        });

        builder.Navigation(p => p.Items)
            .HasField(SchemaConstants.BackingFields.DuesPeriodItems)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(p => p.DomainEvents);
    }
}
