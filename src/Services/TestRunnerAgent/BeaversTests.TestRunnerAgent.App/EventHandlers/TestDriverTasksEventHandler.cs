using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Core;
using BeaversTests.TestRunnerController.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.App.EventHandlers;

public class TestDriverTasksEventHandler(
    TestRunnerContext testRunnerContext,
    ILogger<TestDriverTasksEventHandler> logger) : IEventHandler<TestDriverValidationTaskEvent>
{
    public async Task Handle(TestDriverValidationTaskEvent notification, CancellationToken cancellationToken)
    {
        if (notification.TestAgentId != testRunnerContext.Id)
            return;
        
        logger.LogDebug("Test driver validation task event received {0} ({1}). Agent id: {2}", 
            notification.Key,
            notification.AgId,
            notification.TestAgentId);

        throw new NotImplementedException();
    }
}