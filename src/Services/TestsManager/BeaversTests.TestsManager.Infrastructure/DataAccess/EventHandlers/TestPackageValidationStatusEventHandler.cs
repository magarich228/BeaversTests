using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.Core.TestPackage;
using BeaversTests.TestsManager.Events.TestPackage;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.EventHandlers;

public class TestPackageValidationStatusEventHandler(
    TestsManagerContext db,
    ILogger<TestPackageValidationStatusEventHandler> logger) : IEventHandler<TestPackageValidationStatusEvent>
{
    public async Task Handle(TestPackageValidationStatusEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Test package {TestPackageId} validation status event has been received.", notification.Id);
        
        await SetValidationStatus(
            notification.Id, 
            Enum.Parse<TestPackageValidationResult>(notification.ValidationStatus), 
            notification.ValidationMessage, 
            cancellationToken);
    }

    private async Task SetValidationStatus(
        Guid testPackageId,
        TestPackageValidationResult validationStatus,
        string validationMessage,
        CancellationToken cancellationToken)
    {
        var testPackage = await db.TestPackages.FindAsync([testPackageId], cancellationToken);

        if (testPackage == null)
        {
            throw new TestsManagerInfrastructureException("Test package not found.");
        }
        
        testPackage.ValidationStatus = validationStatus;
        testPackage.ValidationMessage = validationMessage;

        var rows = await db.SaveChangesAsync(cancellationToken);

        if (rows == 0)
        {
            throw new TestsManagerInfrastructureException("Test package not updated.");
        }
    }
}