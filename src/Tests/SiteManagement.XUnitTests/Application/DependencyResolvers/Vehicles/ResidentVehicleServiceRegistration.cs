using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Vehicles;

public static class ResidentVehicleServiceRegistration
{
    public static void AddResidentVehicleServices(this IServiceCollection services)
    {

        //Create
        services.AddTransient<CreateResidentVehicleCommand> ();
    }
}
