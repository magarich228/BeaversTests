using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Events;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.EventHandlers;

public class TestRunnerPreparedEventHandlerReadDb(ITestRunnerControllerContext db) : IEventHandler<TestRunnerPreparedEvent>
{
    public async Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        var testAgent = new TestAgent()
        {
            Id = notification.Id,
            Status = TestAgentStatus.Prepared
        };

        var addedAgent = await db.TestAgents.AddAsync(testAgent, cancellationToken);
        var rows = await db.SaveChangesAsync(cancellationToken);

        // TODO: custom exception
        if (rows == 0)
            throw new ApplicationException($"Failed to add test agent {addedAgent.Entity.Id} to controller context.");
    }
}