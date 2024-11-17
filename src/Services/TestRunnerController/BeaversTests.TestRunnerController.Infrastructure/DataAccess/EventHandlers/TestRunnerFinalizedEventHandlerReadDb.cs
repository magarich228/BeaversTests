using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.EventHandlers;

public class TestRunnerFinalizedEventHandlerReadDb(
    ITestRunnerControllerContext db,
    ILogger<TestRunnerFinalizedEventHandlerReadDb> logger) 
    : IEventHandler<TestRunnerFinalizedEvent>
{
    public async Task Handle(TestRunnerFinalizedEvent notification, CancellationToken cancellationToken)
    {
        var testAgent = await db.TestAgents.FindAsync([notification.Id], cancellationToken);

        if (testAgent == null)
        {
            logger.LogWarning("Finalized test agent {Notification} not found in controller context.", notification.Id);
            return;
        }

        var removedAgent =  db.TestAgents.Remove(testAgent);
        
        if ((await db.SaveChangesAsync(cancellationToken)) == 0)
            throw new ApplicationException($"Failed to remove test agent {removedAgent.Entity.Id} from controller context.");
    }
}