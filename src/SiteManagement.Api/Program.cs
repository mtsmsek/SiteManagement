using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI JSON document at /openapi/v1.json
    app.MapOpenApi();

    // Scalar UI at /scalar (W1 Day 4'te JWT bearer scheme eklenecek)
    app.MapScalarApiReference(options =>
    {
        options.Title = "SiteManagement API";
    });
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();

public partial class Program;
