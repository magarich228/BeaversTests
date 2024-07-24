using BeaversTests.Common.CQRS;
using BeaversTests.TestsManager.Api;
using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.Infrastructure;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
    
    //c.IncludeXmlComments();
});

services.AddTestsManagerInfrastructure(configuration);
services.AddTestsManagerApp();
services.AddCqrsBusses();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseApi();

using (var scope = app.Services.CreateScope())
{
    await using var db = scope.ServiceProvider.GetRequiredService<TestsManagerContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();