using BeaversTests.Auth.AspNetCore.Shared;
using BeaversTests.Auth.Bundle;
using BeaversTests.Manager.Persistence;
using BeaversTests.Manager.Persistence.Dal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddPersistence(configuration);

services.AddHttpContextAccessor();

services.AddAuthModule(configuration);
services.AddBeaversTestsAuth(configuration);

services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation());

services.AddControllers();

if (!builder.Environment.IsProduction())
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "BeaversTests Manager", Version = "v1" });

        c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter 'Bearer' [space] and then your valid JWT token"
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });
}

const string allowAllPolicyName = "AllowAll";

services.AddCors(options =>
{
    options.AddPolicy(allowAllPolicyName,
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await using var db = scope.ServiceProvider.GetRequiredService<TestsManagerDbContext>();
    await db.Database.MigrateAsync();
}

app.UseCors(allowAllPolicyName);

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BeaversTests Auth API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseBeaversTestsAuth();

app.MapControllers();

app.Run();

// TODO: OpenTelemetry, OpenApiYaml endpoint check