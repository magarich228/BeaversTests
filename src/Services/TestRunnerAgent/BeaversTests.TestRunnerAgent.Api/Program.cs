using BeaversTests.Common.Binary;
using BeaversTests.TestRunner;
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

    using var runnerClient = scope.ServiceProvider.GetRequiredService<RunnerClient>();
    
    var bytes = "Hello world!"u8.ToArray();
    await runnerClient.SendAsync(new DriverValidationCommand()
    {
        DriverKey = "Test",
        AgId = Guid.NewGuid(),
        Driver = new TestDriverContent()
        {
            Files = new List<BeaversTestsFile>()
            {
                new BeaversTestsFile()
                {
                    Content = bytes,
                    Length = bytes.Length,
                    Name = "testfile",
                    MediaType = "application/octet-stream"
                }
            }
        }
    });
}

await app.RunAsync();

// TODO: разгрести референсы для сервисов, удалить лишние Nuget пакеты