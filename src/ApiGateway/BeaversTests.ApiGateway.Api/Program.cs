using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    // .AddJsonFile("testsManager.ocelot.json", optional: false, reloadOnChange: true)
    // .AddJsonFile("identity.ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// services.AddSwaggerGen();
services.AddEndpointsApiExplorer();
services.AddOcelot(configuration);
services.AddSwaggerForOcelot(configuration);

// services.AddEndpointsApiExplorer();
// services.AddSwaggerGen();

var app = builder.Build();

await app.UseSwaggerForOcelotUI()
    .UseOcelot();

// app.UseSwagger();
// app.UseSwaggerUI();

await app.RunAsync();