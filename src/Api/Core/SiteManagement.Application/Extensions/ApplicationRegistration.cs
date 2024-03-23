using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SiteManagement.Application.CrossCuttingConcerns.Caching;
using SiteManagement.Application.Pipelines.Authorization;
using SiteManagement.Application.Pipelines.Validation;
using SiteManagement.Application.Rules.Commons;
using SiteManagement.Application.Security.JWT;
using System.Reflection;

namespace SiteManagement.Application.Extensions
{
    public static class ApplicationRegistration
    {
        public static IServiceCollection AddApplicationRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            var assemblies = Assembly.GetExecutingAssembly();
            services.AddAutoMapper(assemblies);

            services.addSubClassesOfType(assemblies, typeof(BaseBusinessRules));
            services.AddValidatorsFromAssembly(assemblies);

            services.AddMediatR(conf =>
            {
                conf.RegisterServicesFromAssemblies(assemblies);

                conf.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
                conf.AddOpenBehavior(typeof(CachingBehavior<,>));
                conf.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
                conf.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            });

            services.AddScoped<ITokenHelper, JwtHelper>();
            

            return services;    
        }

        private static IServiceCollection addSubClassesOfType(this IServiceCollection services,
                                                              Assembly assembly,
                                                               Type type,
                                                               Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null) 
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
            foreach (var item in types)
            {
                if (addWithLifeCycle == null)
                    services.AddScoped(item);

                else
                    addWithLifeCycle(services,item);

            }
            return services;
        }
    }
}
