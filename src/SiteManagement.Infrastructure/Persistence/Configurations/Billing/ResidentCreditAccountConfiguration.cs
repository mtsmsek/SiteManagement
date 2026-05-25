using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Billing;
using SiteManagement.Infrastructure.Persistence.Converters;

namespace SiteManagement.Infrastructure.Persistence.Configurations.Billing;

/// <summary>
/// EF mapping for the <see cref="ResidentCreditAccount"/> aggregate root: a flat
/// row holding a resident's credit balance as a Money-as-decimal column. A
/// unique index on <c>ResidentId</c> enforces one account per resident.
/// </summary>
public sealed class ResidentCreditAccountConfiguration : IEntityTypeConfiguration<ResidentCreditAccount>
{
    public void Configure(EntityTypeBuilder<ResidentCreditAccount> builder)
    {
        builder.ToTable(SchemaConstants.Tables.ResidentCreditAccounts);

        builder.HasKey(a => a.Id);

        builder.Property<uint>(SchemaConstants.ConcurrencyTokenColumn)
            .HasColumnName(SchemaConstants.ConcurrencyTokenColumn)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();

        builder.Property(a => a.ResidentId)
            .HasColumnName(SchemaConstants.Columns.ResidentId)
            .IsRequired();

        builder.Property(a => a.Balance)
            .HasConversion(new MoneyConverter())
            .HasColumnName(SchemaConstants.Columns.Amount)
            .HasPrecision(BillingLimits.MoneyPrecision, BillingLimits.MoneyScale)
            .IsRequired();

        builder.HasIndex(a => a.ResidentId).IsUnique();

        builder.Ignore(a => a.DomainEvents);
    }
}
