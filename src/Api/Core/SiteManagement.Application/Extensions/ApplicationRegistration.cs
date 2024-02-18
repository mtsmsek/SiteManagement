using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Application.Pipelines.Validation;
using System.Reflection;

namespace SiteManagement.Application.Extensions
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)
        {
            var assemblies = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(assemblies);
            services.AddValidatorsFromAssembly(assemblies);

            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(assemblies);

                conf.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
                conf.AddOpenBehavior(typeof(CachingBehavior<,>));
                conf.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
            });

            return services;    
        }
    }
}
