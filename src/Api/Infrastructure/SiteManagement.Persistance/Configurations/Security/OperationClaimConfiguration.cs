using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Security;

public class OperationClaimConfiguration : BaseEntityConfiguration<OperationClaim>
{
    public override void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.Property(operationClaim => operationClaim.Name).IsRequired();
    }
}
