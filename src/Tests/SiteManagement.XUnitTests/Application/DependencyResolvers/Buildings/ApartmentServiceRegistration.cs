using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Buildings.Apartments.CreateApartment;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Buildings;

public static class ApartmentServiceRegistration
{
    public static void AddApartmentServices(this IServiceCollection services)
    {
        services.AddTransient<ApartmentFakeDatas>();
        services.AddTransient<CreateApartmentCommand>();
        services.AddTransient<CreateApartmentCommandValidator>();

    }
}
