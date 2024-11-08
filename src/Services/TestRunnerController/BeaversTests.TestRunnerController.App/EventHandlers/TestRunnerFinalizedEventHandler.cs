using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.App.Abstractions;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerFinalizedEventHandler(ITestRunnerControllerContext db) : IEventHandler<TestRunnerFinalizedEvent>
{
    public async Task Handle(TestRunnerFinalizedEvent notification, CancellationToken cancellationToken)
    {
        var testAgent = await db.TestAgents.FindAsync([notification.Id], cancellationToken);
        
        if (testAgent == null)
            throw new ApplicationException("Finalized test agent not found in controller context.");
        
        var removedAgent =  db.TestAgents.Remove(testAgent);
        
        if ((await db.SaveChangesAsync(cancellationToken)) == 0)
            throw new ApplicationException($"Failed to remove test agent {removedAgent.Entity.Id} from controller context.");
    }
}