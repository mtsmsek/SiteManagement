using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Residents;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Residents
{
    public static class ResidentServiceRegistration
    {
        public static void AddResidentServices(this IServiceCollection services)
        {
            //Fake data
            services.AddTransient<ResidentFakeDatas>();
            //Create Resident
            services.AddTransient<CreateResidentCommand>();
            services.AddTransient<CreateResidentValidator>();

            //Login
            services.AddTransient<ResidentLoginCommand>();
            services.AddTransient<ResidentLoginCommandValidator>();

        }
    }
}
