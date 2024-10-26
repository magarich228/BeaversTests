using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerFinalizedEventHandler(AgentsContext agentsContext) : IEventHandler<TestRunnerFinalizedEvent>
{
    public Task Handle(TestRunnerFinalizedEvent notification, CancellationToken cancellationToken)
    {
        if (!agentsContext.TestAgents.TryRemove(notification.Id, out _))
            throw new ApplicationException("Finalized test agent not found in controller context.");
        
        return Task.CompletedTask;
    }
}