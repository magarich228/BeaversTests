using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Core;
using BeaversTests.TestRunnerAgent.Core.Tasks;
using BeaversTests.TestRunnerController.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.App.EventHandlers;

public class TestDriverTasksEventHandler(
    TasksContainer tasksContainer,
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

        tasksContainer.Register(new DriverValidationTask()
        {
            AgId = notification.AgId,
            DriverKey = notification.Key,
            UserId = notification.UserId
        });
    }
}