using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.UnitTest.Mock.FakeDatas.Buildings;

namespace SiteManagement.UnitTest.DependencyResolvers.Buildings
{
    public static class BlockServiceRegistration
    {
        public static void AddBlockServices(this IServiceCollection services)
        {
            services.AddTransient<BlockFakeDatas>();
            services.AddTransient<CreateBlockCommand>();
        }
    }
}
