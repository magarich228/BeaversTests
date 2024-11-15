using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Core;
using BeaversTests.TestRunnerController.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.App.EventHandlers;

public class TestPackageValidationTaskEventHandler(
    ILogger<TestPackageValidationTaskEventHandler> logger) : IEventHandler<TestPackageValidationTaskEvent>
{
    public Task Handle(TestPackageValidationTaskEvent notification, CancellationToken cancellationToken)
    {
        if (notification.TestAgentId != TestRunnerContext.Id)
        {
            return Task.CompletedTask;
        }
        
        logger.LogInformation("Test package {TestPackageId} validation task received.", notification.Id);

        // TODO: Протестить события валидации тестов.
        // TODO: посмотреть и разделить TestDrivers либу на регистрацию и резолв.
        throw new NotImplementedException();
    }
}