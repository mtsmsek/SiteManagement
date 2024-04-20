using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Payments;

public class CreditCardConfiguration:BaseEntityConfiguration<CreditCard>
{
    public override void Configure(EntityTypeBuilder<CreditCard> builder)
    {

        builder.Property(x => x.CardNumber).IsRequired();
        builder.Property(x => x.CVCNumber).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.NameOnCard).IsRequired();
        builder.Property(x => x.ExpireDate).IsRequired();

    }
}
