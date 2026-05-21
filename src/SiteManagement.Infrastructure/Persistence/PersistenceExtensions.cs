using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Abstractions.Events;
using SiteManagement.Application.Abstractions.Persistence;
using SiteManagement.Application.Billing.Queries;
using SiteManagement.Application.Property.Queries;
using SiteManagement.Application.Residency.Queries;
using SiteManagement.Application.Tenancy.Queries;
using SiteManagement.Infrastructure.Events;
using SiteManagement.Infrastructure.Persistence.Queries;
using SiteManagement.Infrastructure.Persistence.Repositories;

namespace SiteManagement.Infrastructure.Persistence;

/// <summary>
/// Persistence wiring: <see cref="AppDbContext"/>, the command-side
/// repositories, the unit of work, and the read-side query services. Kept
/// next to the rest of the Persistence folder so the EF surface lives in
/// one place and <c>AddInfrastructure</c> stays small.
/// </summary>
public static class PersistenceExtensions
{
    /// <summary>Configuration key for the relational connection string.</summary>
    public const string ConnectionStringName = "DefaultConnection";

    /// <summary>
    /// Registers EF Core + Postgres, repositories, unit of work, and read
    /// query services on the supplied collection.
    /// </summary>
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName)
            ?? throw new InvalidOperationException(
                $"Connection string '{ConnectionStringName}' is missing from configuration.");

        services.AddDbContext<AppDbContext>(opts => opts.UseNpgsql(
            connectionString,
            npg => npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // Domain events: aggregates raise them, the unit of work flushes them
        // through this dispatcher after each save.
        services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

        // Command-side: aggregate repositories + unit of work.
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<ISiteRepository, SiteRepository>();
        services.AddScoped<IResidentRepository, ResidentRepository>();
        services.AddScoped<IApartmentAssignmentRepository, ApartmentAssignmentRepository>();
        services.AddScoped<IDuesPeriodRepository, DuesPeriodRepository>();
        services.AddScoped<IUtilityBillPeriodRepository, UtilityBillPeriodRepository>();

        // Read-side: per-context query services, scoped to the same DbContext.
        services.AddScoped<ISiteQueries, SiteQueries>();
        services.AddScoped<IResidentQueries, ResidentQueries>();
        services.AddScoped<ITenancyQueries, TenancyQueries>();
        services.AddScoped<IBillingQueries, BillingQueries>();

        return services;
    }
}
