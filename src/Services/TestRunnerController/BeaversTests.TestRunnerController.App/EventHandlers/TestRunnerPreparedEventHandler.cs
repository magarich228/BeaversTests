using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerPreparedEventHandler(
    IEventBus eventBus,
    ILogger<TestRunnerPreparedEventHandler> logger) 
    : IEventHandler<TestRunnerPreparedEvent>
{
    public async Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("TestRunnerPreparedEventHandler: handled {Notification}", notification.Id);
        
        await eventBus.CommitLocalAsync(
            cancellationToken, 
            new BeaversTests.TestRunnerController.Events.TestRunnerPreparedEvent()
        {
            Id = notification.Id
        });
    }
}