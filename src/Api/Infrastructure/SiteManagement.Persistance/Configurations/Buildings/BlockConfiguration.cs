using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Persistance.Configurations.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Buildings
{
    public class BlockConfiguration : BaseEntityConfiguration<Block>
    {
        public override void Configure(EntityTypeBuilder<Block> builder)
        {
            builder.Property(block => block.Name).IsRequired();

        }
    }
}
