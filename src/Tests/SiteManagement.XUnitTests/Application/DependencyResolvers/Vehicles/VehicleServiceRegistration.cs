using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
using SiteManagement.Application.Features.Commands.Vehicles.DeleteCehicle.HardDelete;
using SiteManagement.Application.Features.Commands.Vehicles.UpdateVehicle;
using SiteManagement.Application.Features.Queries.Vehicles.GetListVehicles;
using SiteManagement.Application.Features.Queries.Vehicles.GetVehicleByRegistrationPlate;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Vehicles
{
    public static class VehicleServiceRegistration
    {
        public static void AddVehicleServices(this IServiceCollection services)
        {
            //Fake data
            services.AddTransient<VehicleFakeData>();
            //Create
            services.AddTransient<CreateVehicleCommand>();
            services.AddTransient<CreateVehicleCommandValidator>();

            //HardDelete
            services.AddTransient<HardDeleteVehicleCommand>();

            //Update
            services.AddTransient<UpdateVehicleCommand>();
            services.AddTransient<UpdateVehicleCommandValidator>();

            //Get List All Vehicles
            services.AddTransient<GetListAllVehiclesQuery>();

            //Get Vehicle By Registration Plate 
            services.AddTransient<GetVehicleByRegistrationPlateQuery>();
            

        }
    }
}
