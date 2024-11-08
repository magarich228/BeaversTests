using BeaversTests.Postgres.EventStore;
using BeaversTests.TestRunnerController.Api;
using BeaversTests.TestRunnerController.App;
using BeaversTests.TestRunnerController.Infrastructure;
using BeaversTests.TestRunnerController.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

services.AddTestRunnerControllerInfrastructure(configuration);
services.AddTestRunnerControllerApp(configuration);
services.AddApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.UseApi();

using (var scope = app.Services.CreateScope())
{
    await using var db = scope.ServiceProvider.GetRequiredService<TestRunnerControllerContext>();
    await db.Database.MigrateAsync();
    
    // TODO: отказаться от указания конкретного типа PostgresEventStore
    await using var eventStore = scope.ServiceProvider.GetRequiredService<PostgresEventStore>();
    await eventStore.Database.MigrateAsync();
}

await app.RunAsync();