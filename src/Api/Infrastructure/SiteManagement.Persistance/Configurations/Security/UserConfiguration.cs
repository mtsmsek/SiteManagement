using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Domain.Enumarations.Security;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Security;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.Property(user => user.FirstName).IsRequired();
        builder.Property(user => user.LastName).IsRequired();
        builder.Property(user => user.Email).IsRequired();
        builder.Property(user => user.PasswordSalt).IsRequired();
        builder.Property(user => user.PasswordHash).IsRequired();
        builder.Property(user => user.AuthenticatorType).HasConversion(userType =>
                                                          userType.Value,
                                                          value => AuthenticatorType.FromValue(value)!).IsRequired();


    }
}
