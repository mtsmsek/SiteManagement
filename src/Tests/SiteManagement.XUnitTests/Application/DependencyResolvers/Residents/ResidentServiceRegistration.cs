using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Residents.CreateResident;
using SiteManagement.Application.Features.Commands.Residents.DeleteResident.HardDelete;
using SiteManagement.Application.Features.Commands.Residents.Login;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateEmail;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdateInformation;
using SiteManagement.Application.Features.Commands.Residents.UpdateResident.UpdatePassword;
using SiteManagement.Application.Features.Queries.Residents.GetListAllResidents;
using SiteManagement.Application.Features.Queries.Residents.GetListResidentByApartmentNumberAndBlockName;
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

            //HardDelete
            services.AddTransient<HardDeleteResidentCommand>();

            //UpdateEmail
            services.AddTransient<UpdateResidentEmailCommand>();
            services.AddTransient<UpdateResidentEmailCommandValidator>();

            //Update Password
            services.AddTransient<UpdateResidentPasswordCommand>();
            services.AddTransient<UpdateResidentPasswordCommandValidator>();

            //Update Information
            services.AddTransient<UpdateResidentCommand>();
            services.AddTransient<UpdateResidentCommandValidator>();


            //Queries
            //Get List All Residnets
            services.AddTransient<GetListAllResidentsQuery>();

            //Get List Residents By ApartmentNumber And Block Name
            services.AddTransient<GetListResidentsByApartmentNumberAndBlockNameQuery>();
        }
    }
}
