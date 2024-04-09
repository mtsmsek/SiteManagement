using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Contexts.SeedDatas;
using SiteManagement.Persistance.Services.Repositories.Buildings;
using SiteManagement.Persistance.Services.Repositories.Invoices;
using SiteManagement.Persistance.Services.Repositories.Residents;
using SiteManagement.Persistance.Services.Repositories.Vehicles;
using SiteManagement.Application.Services.Repositories.Buildings;
using SiteManagement.Application.Services.Repositories.Invoices;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Application.Services.Repositories.Vehicles;
using SiteManagement.Application.Services.Repositories.Security;
using SiteManagement.Persistance.Services.Repositories.Security;
using SiteManagement.Application.Services.Messages;
using SiteManagement.Persistance.Services.Messages;


namespace SiteManagement.Persistance.Extensions;

public static class PersistanceRegistration
{
    public static IServiceCollection AddPersistanceExtensions(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddDbContext<SiteManagementApplicationContext>(conf =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnectionString");
            conf.UseSqlServer(connectionString, opt =>
            {
                opt.EnableRetryOnFailure();
            });

        });
        
        #region SeedData
        var seedData = new SiteManagementSeedData();
        seedData.SeedAsync(configuration).GetAwaiter().GetResult();
        #endregion
        #region Buildings 
        services.AddScoped<IApartmentRepository, ApartmentRepository>();
        services.AddScoped<IBlockRepository, BlockRepository>();
        #endregion
        #region Invoices
        services.AddScoped<IBillReposiotry, BillRepository>();
        #endregion
        #region Residents
        services.AddScoped<IResidentRepository, ResidentRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddScoped<IMessageService, MessageManager>();

        #endregion
        #region Vehicles
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IResidentVehicleRepository, ResidentVehicleRepository>();
        #endregion
        #region Security
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserOperationClaimRepository, UserOperationClaimRepository>();
        services.AddScoped<IOperationClaimRepository, OperationClaimRepository>();
        services.AddScoped<IEmailAuthenticatorRepository, EmailAuthenticatorRepository>();
        #endregion
        return services;
    }
    public static IServiceCollection AddPersistanceManager(this IServiceCollection services)
    {
        services.AddScoped<IMessageService, MessageManager>();
        return services;
    }
}
