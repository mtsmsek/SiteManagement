using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagemnt.Application.Extensions
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationRegistration(IServiceCollection services)
        {
            //TODO - Add mediatr dependency injection
            return services;    
        }
    }
}
