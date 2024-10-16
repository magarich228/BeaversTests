using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core.TestPackage;
using BeaversTests.TestsManager.Events.TestPackage;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.EventHandlers;

public class TestPackageAddedEventHandler(ITestsManagerContext db) : IEventHandler<TestPackageAddedEvent>
{
    public async Task Handle(TestPackageAddedEvent notification, CancellationToken cancellationToken)
    {
        // TODO: map?
        var testPackage = new BeaversTestPackage()
        {
            Id = notification.Id,
            Name = notification.Name,
            Description = notification.Description,
            TestDriverKey = notification.TestDriverKey,
            TestProjectId = notification.TestProjectId
        };
        
        await db.TestPackages.AddAsync(testPackage, cancellationToken);
        
        if (await db.SaveChangesAsync(cancellationToken) < 1)
        {
            throw new TestsManagerException("Test package not added.");
        }
    }
}