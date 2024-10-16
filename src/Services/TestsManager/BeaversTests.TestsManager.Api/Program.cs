using BeaversTests.Postgres.EventStore;
using BeaversTests.TestsManager.Api;
using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.Infrastructure;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

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
services.AddTestsManagerApp(configuration);
services.AddApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseApi();

using (var scope = app.Services.CreateScope())
{
    await using var db = scope.ServiceProvider.GetRequiredService<TestsManagerContext>();
    await db.Database.MigrateAsync();
    
    await using var eventStore = scope.ServiceProvider.GetRequiredService<PostgresEventStore>();
    await eventStore.Database.MigrateAsync();
}

await app.RunAsync();