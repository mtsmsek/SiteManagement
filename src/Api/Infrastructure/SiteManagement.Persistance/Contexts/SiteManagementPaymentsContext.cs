using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Contexts
{
    public class SiteManagementPaymentsContext : DbContext
    {
        public DbSet<CreditCard> CreditCards { get; set; }
        public SiteManagementPaymentsContext()
        {

        }
        public SiteManagementPaymentsContext(DbContextOptions<SiteManagementPaymentsContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO - write password before running
            if (!optionsBuilder.IsConfigured)
            {

                var connStr = "Host=localhost;Port=5432;Database=Payments;Username=postgres;Password=123456;Pooling=true;";

                optionsBuilder.UseNpgsql(connStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });



            }

        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSave();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void OnBeforeSave()
        {
            var entitiesToAdd = ChangeTracker.Entries()
                                             .Where(entity => entity.State == EntityState.Added)
                                             .Select(entity => (BaseEntity)entity.Entity);

            PrepareAddedEntities(entitiesToAdd);
        }
        private void PrepareAddedEntities(IEnumerable<BaseEntity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity.CreatedDate == DateTime.MinValue)
                    entity.CreatedDate = DateTime.Now;
            }
        }
    }
}
