using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Configurations.Commons
{
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> 
        where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            //TODO -- Add regions
            builder.HasKey(entity => entity.Id);
            builder.Property(entity => entity.CreatedDate).IsRequired();
            builder.Property(entity => entity.UpdatedDate).IsRequired();
        }
    }
}
