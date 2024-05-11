using Microsoft.Extensions.DependencyInjection;
using SiteManagement.XUnitTests.Application.DependencyResolvers.Buildings;
using SiteManagement.XUnitTests.Application.DependencyResolvers.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.XUnitTests
{
    public sealed class Startup
    {
        public IServiceCollection Services { get; private set; }


        public Startup()
        {
            Services = new ServiceCollection();

        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBlockServices();
            services.AddApartmentServices();
            services.AddBillServices();

        }
    }
}
