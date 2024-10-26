using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.Core;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerPreparedEventHandler(AgentsContext agentsContext) : IEventHandler<TestRunnerPreparedEvent>
{
    public Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        var agent = new TestAgent()
        {
            Id = notification.Id,
            Status = TestAgentStatus.Prepared
        };

        // TODO: custom exception
        if (!agentsContext.TestAgents.TryAdd(agent.Id, agent))
            throw new ApplicationException("Test agent already exists in controller context.");
        
        return Task.CompletedTask;
    }
}