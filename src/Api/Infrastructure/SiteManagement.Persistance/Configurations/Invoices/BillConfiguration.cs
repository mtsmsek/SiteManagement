using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Enumarations.Invoices;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Invoices;

public class BillConfiguration : BaseEntityConfiguration<Bill>
{
    public override void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.Property(bill => bill.ApartmentId).IsRequired();
        builder.Property(bill => bill.Type).HasConversion(billType =>
                                                          billType.Value,
                                                          value => BillType.FromValue(value)!).IsRequired();
        builder.Property(bill => bill.Fee).IsRequired();
        builder.Property(bill => bill.IsPaid).IsRequired();

        builder.Property(bill => bill.Month).HasConversion(month => month.Value,
                                                            value => Month.FromValue(value)!).IsRequired();
        builder.Property(bill => bill.Year).IsRequired();

        builder.HasOne(bill => bill.Apartment)
               .WithMany(apartment => apartment.Bills)
               .HasForeignKey(bill => bill.ApartmentId);
    }
}
