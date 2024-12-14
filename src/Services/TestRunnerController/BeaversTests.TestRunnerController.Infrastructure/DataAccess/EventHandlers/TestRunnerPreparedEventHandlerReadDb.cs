using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.EventHandlers;

public class TestRunnerPreparedEventHandlerReadDb(
    ITestRunnerControllerContext db,
    ILogger<TestRunnerPreparedEventHandlerReadDb> logger) : IEventHandler<TestRunnerPreparedEvent>
{
    public async Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Test agent {Notification} prepared event has been received.", notification.Id);
        
        var testAgent = new TestAgent()
        {
            Id = notification.Id,
            OwnerId = notification.OwnerId,
            Key = notification.Key,
            Status = TestAgentStatus.Prepared
        };

        var addedAgent = await db.TestAgents.AddAsync(testAgent, cancellationToken);
        var rows = await db.SaveChangesAsync(cancellationToken);

        // TODO: custom exception
        if (rows == 0)
            throw new ApplicationException($"Failed to add test agent {addedAgent.Entity.Id} to controller context.");
    }
}