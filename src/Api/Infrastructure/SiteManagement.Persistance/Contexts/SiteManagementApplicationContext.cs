﻿using Microsoft.EntityFrameworkCore;
using SiteManagement.Domain.Entities.Buildings;
using SiteManagement.Domain.Entities.Commons;
using SiteManagement.Domain.Entities.Invoices;
using SiteManagement.Domain.Entities.Payments;
using SiteManagement.Domain.Entities.Residents;
using SiteManagement.Domain.Entities.Security;
using SiteManagement.Domain.Entities.Vehicles;
using System.Reflection;

namespace SiteManagement.Persistance.Contexts;

public class SiteManagementApplicationContext : DbContext
{
    #region Buildings
    public DbSet<Apartment> Apartments { get; set; }
    public DbSet<Block> Blocks { get; set; }
    #endregion
    #region Invoices
    public DbSet<Bill> Bills { get; set; }
    #endregion
    #region Residents
    public DbSet<Resident> Residents { get; set; }
    #endregion
    #region Vehicles
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<ResidentVehicle> ResidentVehicles{ get; set; }
    #endregion
    #region Security
    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }


    #endregion
    #region Payments
    public DbSet<Payment> Payments { get; set; }

    #endregion
    public SiteManagementApplicationContext()
    {
        
    }
    public SiteManagementApplicationContext(DbContextOptions<SiteManagementApplicationContext> options): base(options)
    {
        
    }
    //TODO -Add IsConfiguring
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //TODO - write password before running
        if (!optionsBuilder.IsConfigured)
        {
            //var connStr = "Data Source=localhost;Initial Catalog=SiteManagement;Persist Security Info=True;TrustServerCertificate=true;User ID=SA;Password=Kelt.232323";
            var connStr = "Server =(localdb)\\MSSQLLocalDB;Initial Catalog=siteManagementDb;Integrated Security=False;Trusted_Connection=True;MultipleActiveResultSets=False;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connStr, opt =>
            {
                opt.EnableRetryOnFailure();
            });
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
