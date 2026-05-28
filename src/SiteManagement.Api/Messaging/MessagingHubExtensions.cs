using Microsoft.AspNetCore.Authentication.JwtBearer;
using SiteManagement.Api.Configuration;
using SiteManagement.Application.Messaging.Notifications;

namespace SiteManagement.Api.Messaging;

/// <summary>
/// Wiring for the messaging real-time channel: SignalR core, the JWT
/// query-string handshake (browsers cannot set <c>Authorization</c> headers
/// on a WebSocket upgrade, so the bearer rides on <c>?access_token=...</c>
/// for hub paths only), the <see cref="IMessagingNotifier"/> binding, and the
/// hub endpoint mapping.
/// </summary>
public static class MessagingHubExtensions
{
    /// <summary>
    /// Adds SignalR + binds <see cref="IMessagingNotifier"/> to the SignalR
    /// adapter + patches <see cref="JwtBearerOptions"/> so a JWT carried on
    /// the <c>access_token</c> query string is accepted for hub paths.
    /// </summary>
    public static IServiceCollection AddMessagingHub(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped<IMessagingNotifier, MessagingHubNotifier>();

        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var previous = options.Events?.OnMessageReceived;
            options.Events ??= new JwtBearerEvents();
            options.Events.OnMessageReceived = async ctx =>
            {
                if (previous is not null)
                {
                    await previous(ctx);
                }

                var token = ctx.Request.Query[ApiConstants.SignalRAccessTokenQueryKey];
                var path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(token) && path.StartsWithSegments(ApiConstants.HubsPrefix))
                {
                    ctx.Token = token;
                }
            };
        });

        return services;
    }

    /// <summary>Maps the messaging hub endpoint at <see cref="ApiConstants.MessagingHubPath"/>.</summary>
    public static WebApplication MapMessagingHub(this WebApplication app)
    {
        app.MapHub<MessagingHub>(ApiConstants.MessagingHubPath);
        return app;
    }
}
