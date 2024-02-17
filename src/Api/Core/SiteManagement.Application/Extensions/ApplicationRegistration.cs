using Microsoft.Extensions.DependencyInjection;

namespace SiteManagement.Application.Extensions
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            //TODO - Add mediatr dependency injection
            return services;    
        }
    }
}
