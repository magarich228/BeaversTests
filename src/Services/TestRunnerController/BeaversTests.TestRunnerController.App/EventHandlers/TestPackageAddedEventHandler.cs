using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.Events.TestPackage;

namespace BeaversTests.TestRunnerController.App.EventHandlers;

public class TestPackageAddedEventHandler : IEventHandler<TestPackageAddedEvent>
{
    public Task Handle(TestPackageAddedEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}