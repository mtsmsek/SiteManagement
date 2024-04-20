using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Payments;

public class PaymentConfiguration : BaseEntityConfiguration<Payment>
{
    public override void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.ResidentId).IsRequired();
        builder.Property(x => x.BillId).IsRequired();

        builder.HasOne(x => x.Resident)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.ResidentId);

        builder.HasOne(x => x.Bill)
            .WithMany(x => x.Payments)
            .HasForeignKey(x => x.BillId);
        
    }
}
