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

builder.Services.AddOptions<PaymentService.Api.Configuration.ApiKeyOptions>()
    .Bind(builder.Configuration.GetSection(PaymentService.Api.Configuration.ApiKeyOptions.SectionName));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Service-to-service API key guard (skips /health + docs; no-op when unset).
app.UseMiddleware<ApiKeyMiddleware>();

app.MapHealthChecks("/health");
app.MapControllers();

// Seed the demo card + funded account (idempotent) so the pay flow works out of the box.
await PaymentSeeder.SeedAsync(app.Services);

app.Run();

/// <summary>Exposes <c>Program</c> as a partial class so WebApplicationFactory can target it from E2E tests.</summary>
public partial class Program;
