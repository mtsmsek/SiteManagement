using PaymentService.Api.Configuration;
using PaymentService.Application;
using PaymentService.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPaymentHealthChecks()
    .AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

/// <summary>Exposes <c>Program</c> as a partial class so WebApplicationFactory can target it from E2E tests.</summary>
public partial class Program;
