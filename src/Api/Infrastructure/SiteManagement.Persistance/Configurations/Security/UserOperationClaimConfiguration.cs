using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Persistance.Configurations.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Security
{
    public class UserOperationClaimConfiguration : BaseEntityConfiguration<UserOperationClaim>
    {
        public override void Configure(EntityTypeBuilder<UserOperationClaim> builder)
        {
            builder.Property(uoc => uoc.UserId).IsRequired();
            builder.Property(uoc => uoc.OperationClaimId).IsRequired();

            builder.HasOne(uoc => uoc.User)
                   .WithMany(user => user.UserOperationClaims);

            builder.HasOne(uoc => uoc.OperationClaim)
                    .WithMany(operationClaim => operationClaim.UserOperationClaims);
        }
    }
}
