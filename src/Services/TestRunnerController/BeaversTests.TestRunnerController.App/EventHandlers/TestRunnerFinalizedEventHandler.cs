using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.Core;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerFinalizedEventHandler(
    IEventStore eventStore,
    ILogger<TestRunnerFinalizedEventHandler> logger) : IEventHandler<TestRunnerFinalizedEvent>
{
    public async Task Handle(TestRunnerFinalizedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("TestRunnerFinalizedEventHandler: handled {Notification}", notification.Id);

        var agentAggregate = await eventStore.AggregateStreamAsync<TestAgentAggregate>(new AggregateInfo()
        {
            Id = notification.Id
        }, cancellationToken);
        
        agentAggregate.ApplyFinalized(new Events.TestRunnerFinalizedEvent()
        {
            Id = notification.Id
        });

        await eventStore.StoreAsync(agentAggregate, cancellationToken);
    }
}