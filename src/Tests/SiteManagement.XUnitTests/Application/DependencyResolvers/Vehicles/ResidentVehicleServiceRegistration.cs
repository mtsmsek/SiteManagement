using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete;
using SiteManagement.Application.Features.Commands.VehicleResident.CreateVehicleResident;
using SiteManagement.Application.Features.Commands.VehicleResident.DeleteVehicleResident.HardDelete;
using SiteManagement.Application.Features.Commands.VehicleResident.UpdateVehicleResident;
using SiteManagement.Application.Features.Queries.ResidentVehicles.GetListResidentVehicles;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Vehicles;
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
        //Fake Data 
        services.AddTransient<ResidentVehicleFakeDatas>();
        
        //Create
        services.AddTransient<CreateResidentVehicleCommand>();

        //Hard Delete 
        services.AddTransient<HardDeleteResidentVehicleCommand>();
        //Update
        services.AddTransient<UpdateResidentVehicleCommand>();
        //Get List Resident Vehicle 
        services.AddTransient<GetListResidentVehiclesQuery>();
    }
}
