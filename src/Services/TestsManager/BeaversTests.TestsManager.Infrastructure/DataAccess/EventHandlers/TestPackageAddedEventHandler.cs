using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core;
using BeaversTests.TestsManager.Core.TestPackage;
using BeaversTests.TestsManager.Events.TestPackage;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.EventHandlers;

public class TestPackageAddedEventHandler(
    ITestsManagerContext db,
    ILogger<TestPackageAddedEventHandler> logger) : IEventHandler<TestPackageAddedEvent>
{
    public async Task Handle(TestPackageAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Test package {TestPackageId} added event has been received.", notification.Id);
        
        var testPackage = new BeaversTestPackage()
        {
            Id = notification.Id,
            Name = notification.Name,
            Description = notification.Description,
            TestDriverKey = notification.TestDriverKey,
            TestProjectId = notification.TestProjectId,
            ValidationStatus = ValidationResult.InProgress
        };
        
        await db.TestPackages.AddAsync(testPackage, cancellationToken);
        
        if (await db.SaveChangesAsync(cancellationToken) < 1)
        {
            throw new TestsManagerException("Test package not added.");
        }
    }
}