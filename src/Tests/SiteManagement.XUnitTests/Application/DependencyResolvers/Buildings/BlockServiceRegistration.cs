using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.CreateBlock;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.DeleteBlock.HardDelete;
using SiteManagement.Application.Features.Commands.Buildings.Blocks.UpdateBlock.UpdateBlockName;
using SiteManagement.Application.Features.Queries.Blocks.GetBlockDetailByName;
using SiteManagement.Application.Features.Queries.Blocks.GetListAllBlocks;
using SiteManagement.XUnitTests.Application.Mock.FakeDatas.Buildings;

namespace SiteManagement.XUnitTests.Application.DependencyResolvers.Buildings
{
    public static class BlockServiceRegistration
    {
        public static void AddBlockServices(this IServiceCollection services)
        {
            //Create Block
            services.AddScoped<BlockFakeDatas>();
            services.AddTransient<CreateBlockCommand>();
            services.AddTransient<CreateBlockCommandValidator>();

            //Delete Block
            services.AddTransient<HardDeleteBlockCommand>();

            //Update Block
            services.AddTransient<UpdateBlockNameCommand>();
            services.AddTransient<UpdateBlockNameCommandValidator>();

            //GetBlockDetailByName
            services.AddTransient<GetBlockDetailByNameQuery>();

            //GetAllBlocks
            services.AddTransient<GetListAllBlockQuery>();
        }
    }
}
