using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerAgent.App;
using BeaversTests.TestRunnerAgent.Core;
using BeaversTests.TestRunnerAgent.Events;

namespace BeaversTests.TestRunnerAgent.Api;

public static class LifetimeActions
{
    public static void OnStopping(object? state)
    {
        var app = state as WebApplication ??
                  throw new TestRunnerAgentException("Application not found");
        
        using var scope = app.Services.CreateScope();
    
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        var runnerContext = scope.ServiceProvider.GetRequiredService<TestRunnerContext>();
    
        var finalizedEvent = new TestRunnerFinalizedEvent()
        {
            Id = runnerContext.Id
        };

        eventBus.CommitAsync(default, finalizedEvent);
    }

    public static void OnStarted(object? state)
    {
        var app = state as WebApplication ??
                  throw new TestRunnerAgentException("Application not found");
        
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
}