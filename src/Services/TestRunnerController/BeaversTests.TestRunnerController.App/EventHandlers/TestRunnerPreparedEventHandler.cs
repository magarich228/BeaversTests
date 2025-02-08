using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerPreparedEventHandler(
    ITestRunnerControllerContext db,
    IEventStore eventStore,
    ILogger<TestRunnerPreparedEventHandler> logger) 
    : IEventHandler<TestRunnerPreparedEvent>
{
    public async Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test runner prepared: {Notification}", notification.Id);

        var controllerUserKey = await db.ControllerUserKeys.FirstOrDefaultAsync(
            uk => uk.Key == notification.ControllerConnectionKey, 
            cancellationToken: cancellationToken);

        if (controllerUserKey == null)
        {
            logger.LogDebug($"Controller user key {controllerUserKey} not found.");
            // TODO: ack to agent.
            return;
        }
        
        var agentAggregate = new TestAgentAggregate();
        agentAggregate.ApplyPrepared(new Events.TestRunnerPreparedEvent()
        {
            Id = notification.Id,
            Key = controllerUserKey.Key,
            OwnerId = controllerUserKey.OwnerId
        });

        await eventStore.StoreAsync(agentAggregate, cancellationToken);
    }
}