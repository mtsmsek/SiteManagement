using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Services.Repositories.Buildings;
using SiteManagement.Persistance.Services.Repositories.Invoices;
using SiteManagement.Persistance.Services.Repositories.Residents;
using SiteManagement.Persistance.Services.Repositories.Vehicles;
using SiteManagemnt.Application.Services.Repositories.Buildings;
using SiteManagemnt.Application.Services.Repositories.Invoices;
using SiteManagemnt.Application.Services.Repositories.Residents;
using SiteManagemnt.Application.Services.Repositories.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Persistance.Extensions
{
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
            #region Buildings 
            services.AddScoped<IApartmentRepository, ApartmentRepository>();
            services.AddScoped<IBlockRepository, BlockRepository>();
            #endregion
            #region Invoices
            services.AddScoped<IBillReposiotry, BillRepository>();
            #endregion
            #region Residents
            services.AddScoped<IResidentRepository, ResidentRepository>();
            #endregion
            #region Vehicles
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IResidentVehicleRepository, ResidentVehicleRepository>();
            #endregion
            return services;
        }
    }
}
