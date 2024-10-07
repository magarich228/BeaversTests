using BeaversTests.Common.CQRS;
using BeaversTests.Postgres.EventStore;
using BeaversTests.TestDrivers;
using BeaversTests.TestDrivers.Internal;
using BeaversTests.TestsManager.Api;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.Api.Services;
using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.App.Abstractions;
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
services.AddTestsManagerApp();
services.AddCqrsBusses();
services.AddTestDrivers();
services.AddApi();

// TODO: Вынести
services.AddTransient<ITestPackageContentExtractor<TestPackageZipDto>, ZipTestPackageContentExtractor>();

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