using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.DeleteApartment.HardDelete;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeResidentStatus;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.UpdateApartment.ChangeTenantStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListAllApartmentsByBlock;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByBlockName;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsByStatus;
using SiteManagement.Application.Features.Queries.Apartments.GetListApartmentsInBlockByStatus;
using SiteManagement.XUnitTests.Application.Features.Buildings.Apartments.Queries;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Buildings;

public static class ApartmentServiceRegistration
{
    public static void AddApartmentServices(this IServiceCollection services)
    {
        //Fake data
        services.AddTransient<ApartmentFakeDatas>();
        //Create
        services.AddTransient<CreateApartmentCommand>();
        services.AddTransient<CreateApartmentCommandValidator>();
        //HardDelete
        services.AddTransient<HardDeleteApartmentCommand>();
        //ChangeResidentStatus
        services.AddTransient<ChangeResidentStatusCommand>();
        //ChangeTenantStatus
        services.AddTransient<ChangeTenantStatusCommand>();

        //GetLİstALlApartmentByBlocks
        services.AddTransient<GetListAllApartmentsByBlockQuery>();
        services.AddTransient<GetListAllApartmentsByBlockQueryValidator>();

        //GetListApartmentByBlockName
        services.AddTransient<GetListApartmentsByBlockNameQuery>();

        //GetListApartmentByStatus
        services.AddTransient<GetListApartmentsByStatusQuery>();

        //GetListApartmentsInBlockByStatus
        services.AddTransient<GetListApartmentsInBlockByStatusQuery>();
    }
}
