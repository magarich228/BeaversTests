using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Events;
using BeaversTests.TestsManager.Events.TestPackage;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestPackageAddedEventHandler(
    ILogger<TestPackageAddedEventHandler> logger,
    ITestAgentsPool agentsPool,
    IEventStore eventStore) : IEventHandler<TestPackageAddedEvent>
{
    public async Task Handle(TestPackageAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Test package {TestPackageId} added event has been received.", notification.Id);
        
        var testAgentId = await agentsPool.GetAsync(cancellationToken);
        var tasksAggregate = new TestPackageTasksAggregate();

        if (testAgentId == null)
        {
            var errorEvent = new TestPackageValidationIsNotPossibleEvent()
            {
                Id = notification.Id
            };
            
            tasksAggregate.ApplyValidationIsNotPossible(errorEvent);

            await eventStore.StoreAsync(tasksAggregate, cancellationToken);
        }
        
        var validationTaskEvent = new TestPackageValidationTaskEvent()
        {
            Id = notification.Id,
            TestAgentId = testAgentId!.Value,
            TestDriverKey = notification.TestDriverKey
        };
        
        tasksAggregate.ApplyValidationTask(validationTaskEvent);
        
        await eventStore.StoreAsync(tasksAggregate, cancellationToken);
    }
}