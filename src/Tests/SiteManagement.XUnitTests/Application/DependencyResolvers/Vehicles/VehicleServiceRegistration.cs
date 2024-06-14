using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Vehicles.CreateVehicle;
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
        }
    }
}
