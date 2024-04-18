using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public SiteManagementPaymentsContext()
        {

        }
        public SiteManagementPaymentsContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //TODO - write password before running
            if (!optionsBuilder.IsConfigured)
            {
                //var connStr = "Data Source=localhost;Initial Catalog=SiteManagement;Persist Security Info=True;TrustServerCertificate=true;User ID=SA;Password=Kelt.232323";
                var connStr = "User ID=postgres;Password=123456;Host=localhost;Port=5432;Database= PaymentsDb";

                optionsBuilder.UseSqlServer(connStr, opt =>
                {
                    opt.EnableRetryOnFailure();
                });
                // services.AddDbContext<ECommerceAPIDbContext>(options => options.UseNpgsql(Configuration.ConnectionString), ServiceLifetime.Singleton);

            }

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
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
