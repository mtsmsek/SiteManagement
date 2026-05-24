using PaymentService.Api.Configuration;
using PaymentService.Application;
using PaymentService.Infrastructure;
using PaymentService.Infrastructure.Persistence;
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

// Seed the demo card + funded account (idempotent) so the pay flow works out of the box.
await PaymentSeeder.SeedAsync(app.Services);

app.Run();

/// <summary>Exposes <c>Program</c> as a partial class so WebApplicationFactory can target it from E2E tests.</summary>
public partial class Program;
