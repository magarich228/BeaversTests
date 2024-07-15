using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.Infrastructure;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddTestsManagerInfrastructure(configuration);
services.AddTestsManagerApp();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    await using var db = scope.ServiceProvider.GetRequiredService<TestsManagerContext>();
    await db.Database.MigrateAsync();
}

await app.RunAsync();