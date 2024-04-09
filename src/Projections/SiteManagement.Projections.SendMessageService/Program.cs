using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SiteManagement.Application.Extensions;
using SiteManagement.Application.Services.Messages;
using SiteManagement.Application.Services.Repositories.Residents;
using SiteManagement.Persistance.Contexts;
using SiteManagement.Persistance.Extensions;
using SiteManagement.Persistance.Services.Messages;
using SiteManagement.Persistance.Services.Repositories.Residents;
using SiteManagement.Projections.SendMessageService;
using SiteManagement.Projections.SendMessageService.Services;

var builder = Host.CreateApplicationBuilder(args);

//builder.Services.AddApplicationRegistration(builder.Configuration);



//builder.Services.AddPersistanceExtensions(builder.Configuration);

builder.Services.AddScoped<IMessageService, MessageManager>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddDbContextPool<SiteManagementApplicationContext>(conf =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
    conf.UseSqlServer(connectionString, opt =>
    {
        opt.EnableRetryOnFailure();
    });

});
builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<MessageService>();
var host = builder.Build();
host.Run();




