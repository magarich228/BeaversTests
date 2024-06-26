using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddDbContext<TestsManagerContext>(options =>
    options.UseNpgsql("Host=212.109.198.242;Port=5432;Database=BeaversTests.TestsManager;Username=postgres;Password=***",
        npgOptions => npgOptions.MigrationsAssembly(typeof(TestsManagerContext).Assembly.GetName().Name)));

// services.AddTestsManagerApp();

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