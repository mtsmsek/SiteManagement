using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Configurations.Commons;

namespace SiteManagement.Persistance.Configurations.Buildings;

public class BlockConfiguration : BaseEntityConfiguration<Block>
{
    public override void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.Property(block => block.Name).IsRequired();

        builder.HasIndex(block => block.Name).IsUnique();
    }
}
