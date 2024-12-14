using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestsManager.Events.TestDriver;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestDriverEventsHandler(
    ITestAgentsPool testAgentsPool,
    ILogger<TestDriverEventsHandler> logger) : IEventHandler<TestDriverAddedEvent>
{
    public async Task Handle(TestDriverAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("TestDriverAddedEvent received. Key: {Key}, AgId: {AgId}", notification.Key, notification.AgId);

        var agentId = await testAgentsPool.GetAsync(cancellationToken);
        
        
        
        throw new NotImplementedException();
    }
}