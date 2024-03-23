using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SiteManagement.Application.Security.Encryption;
using SiteManagement.Application.Security.JWT;
using System.Text;

namespace SiteManagement.Api.WebApi.Extensions
{
    public static class AuthRegistrations
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration configuration) 
        {

            TokenOptions? tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Auidience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
                };
            });


            return services;
        }
    }
}
 