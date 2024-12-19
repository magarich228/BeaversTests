using BeaversTests.Isolation;
using BeaversTests.TestRunnerAgent.Api;
using BeaversTests.TestRunnerAgent.App;
using BeaversTests.TestRunnerAgent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddTestRunnerAgentInfrastructure(configuration);
services.AddTestRunnerControllerApp(configuration);
services.AddApi();

services.AddHostedService<TaskEngine>();

var app = builder.Build();

app.MapGet("/", () => "Alive.");
app.UseApi();

using (var scope = app.Services.CreateScope())
{
    var appLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

    appLifetime.ApplicationStopping.Register(LifetimeActions.OnStopping, app);
    appLifetime.ApplicationStarted.Register(LifetimeActions.OnStarted, app);
}

await app.RunAsync();

// TODO: разгрести референсы для сервисов, удалить лишние Nuget пакеты