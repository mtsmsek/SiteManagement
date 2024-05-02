using Microsoft.Extensions.DependencyInjection;
using SiteManagement.UnitTest.DependencyResolvers.Buildings;

namespace SiteManagement.UnitTest;

public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddBlockServices();
     
    }
}
