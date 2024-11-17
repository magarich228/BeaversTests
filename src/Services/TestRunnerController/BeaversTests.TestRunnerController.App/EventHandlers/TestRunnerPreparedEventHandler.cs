using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.Core;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerPreparedEventHandler(
    IEventStore eventStore,
    ILogger<TestRunnerPreparedEventHandler> logger) 
    : IEventHandler<TestRunnerPreparedEvent>
{
    public async Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("TestRunnerPreparedEventHandler: handled {Notification}", notification.Id);
        
        var agentAggregate = new TestAgentAggregate();
        agentAggregate.ApplyPrepared(new Events.TestRunnerPreparedEvent()
        {
            Id = notification.Id
        });

        await eventStore.StoreAsync(agentAggregate, cancellationToken);
    }
}