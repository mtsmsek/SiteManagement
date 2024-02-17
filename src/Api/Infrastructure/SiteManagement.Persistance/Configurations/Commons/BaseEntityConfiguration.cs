using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SiteManagement.Domain.Entities.Commons;

namespace SiteManagement.Persistance.Configurations.Commons;

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
