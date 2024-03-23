using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Security
{
    public class EmailAuthenticatorConfiguration : BaseEntityConfiguration<EmailAuthenticator>
    {
        public override void Configure(EntityTypeBuilder<EmailAuthenticator> builder)
        {
            builder.Property(ea => ea.UserId).IsRequired();
            builder.Property(ea => ea.ActivationKey);
            builder.Property(ea => ea.IsVerified).IsRequired();

            builder.HasOne(ea => ea.User)
                    .WithMany(ea => ea.EmailAuthenticators);
        }
    }
}
