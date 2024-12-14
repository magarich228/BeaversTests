using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerAgent.Api;
using BeaversTests.TestRunnerAgent.App;
using BeaversTests.TestRunnerAgent.Core;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerAgent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddTestRunnerAgentInfrastructure(configuration);
services.AddTestRunnerControllerApp(configuration);
services.AddApi();

var app = builder.Build();

app.MapGet("/", () => "Alive.");
app.UseApi();

using (var scope = app.Services.CreateScope())
{
    var appLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();
    
    appLifetime.ApplicationStopping.Register(OnStopping);
    appLifetime.ApplicationStarted.Register(OnStarted);
}

await app.RunAsync();

void OnStopping()
{
    using var scope = app.Services.CreateScope();
    
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    var runnerContext = scope.ServiceProvider.GetRequiredService<TestRunnerContext>();
    
    var finalizedEvent = new TestRunnerFinalizedEvent()
    {
        Id = runnerContext.Id
    };

    eventBus.CommitAsync(default, finalizedEvent);
}

void OnStarted()
{
    using var scope = app.Services.CreateScope();
    
    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
    var runnerContext = scope.ServiceProvider.GetRequiredService<TestRunnerContext>();

    var preparedEvent = new TestRunnerPreparedEvent()
    {
        Id = runnerContext.Id,
        ControllerConnectionKey = runnerContext.ControllerConnectionKey
    }; // createdEvent?
    
    eventBus.CommitAsync(default, preparedEvent);
}

// TODO: подумать над окружениями для тестов в агенте
// TODO: разгрести референсы для сервисов, удалить лишние Nuget пакеты