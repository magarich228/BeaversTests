using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestRunnerPreparedEventHandler(IEventBus eventBus) : IEventHandler<TestRunnerPreparedEvent>
{
    public async Task Handle(TestRunnerPreparedEvent notification, CancellationToken cancellationToken)
    {
        await eventBus.CommitLocalAsync(
            cancellationToken, 
            new BeaversTests.TestRunnerController.Events.TestRunnerPreparedEvent()
        {
            Id = notification.Id
        });
    }
}