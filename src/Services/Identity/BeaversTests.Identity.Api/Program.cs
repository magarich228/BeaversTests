using BeaversTests.Api.Shared;
using BeaversTests.Identity.Api.Firebase;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddAuthInternal()
    .AddAuth();

services.AddControllers();

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.UseAuth();

await app.RunAsync();