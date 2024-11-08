using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerFinalizedEventHandler(IEventBus eventBus) : IEventHandler<TestRunnerFinalizedEvent>
{
    public async Task Handle(TestRunnerFinalizedEvent notification, CancellationToken cancellationToken)
    {
        await eventBus.CommitLocalAsync(
            cancellationToken, 
            new BeaversTests.TestRunnerController.Events.TestRunnerFinalizedEvent()
            {
                Id = notification.Id
            });
    }
}