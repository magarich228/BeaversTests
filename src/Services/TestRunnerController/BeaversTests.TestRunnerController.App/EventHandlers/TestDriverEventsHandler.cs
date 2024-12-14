using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Events;
using BeaversTests.TestsManager.Events.TestDriver;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestDriverEventsHandler(
    IEventBus eventBus,
    ITestAgentsPool testAgentsPool,
    ILogger<TestDriverEventsHandler> logger) : IEventHandler<TestDriverAddedEvent>
{
    public async Task Handle(TestDriverAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("TestDriverAddedEvent received. Key: {Key}, AgId: {AgId}", notification.Key, notification.AgId);

        var agentId = await testAgentsPool.GetAsync(cancellationToken);

        if (agentId is null)
        {
            var validationIsNotPossibleEvent = new TestDriverValidationIsNotPossibleEvent()
            {
                Key = notification.Key,
                AgId = notification.AgId
            };
            
            await eventBus.CommitAsync(cancellationToken, validationIsNotPossibleEvent);

            logger.LogDebug("TestDriverValidationIsNotPossibleEvent was sent. Key: {Key}, AgId: {AgId}", notification.Key, notification.AgId);
            
            return;
        }

        var validationTaskEvent = new TestDriverValidationTaskEvent()
        {
            Key = notification.Key,
            AgId = notification.AgId,
            TestAgentId = agentId.Value
        };

        await eventBus.CommitAsync(cancellationToken, validationTaskEvent);

        logger.LogDebug("TestDriverValidationTaskEvent was sent. Key: {Key}, AgId: {AgId}", notification.Key, notification.AgId);
    }
}