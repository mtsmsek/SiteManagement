using SiteManagement.Persistance.Extensions;
using SiteManagement.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region Extensions
builder.Services.AddApplicationRegistration();

builder.Services.AddPersistanceExtensions(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddLogging(conf => conf.AddConsole());

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
