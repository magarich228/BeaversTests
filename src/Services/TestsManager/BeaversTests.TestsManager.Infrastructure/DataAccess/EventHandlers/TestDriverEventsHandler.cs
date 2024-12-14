using AutoMapper;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.Core;
using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.EventHandlers;

public class TestDriverEventsHandler(
    TestsManagerContext db,
    IMapper mapper,
    ILogger<TestDriverEventsHandler> logger) : 
    IEventHandler<TestDriverAddedEvent>, 
    IEventHandler<TestDriverRemovedEvent>,
    IEventHandler<TestDriverValidationStatusEvent>
{
    public async Task Handle(TestDriverAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test driver {TestDriverKey} added event has been received.", notification.Key);

        var driver = mapper.Map<TestDriverAddedEvent, TestDriver>(notification);

        await db.TestDrivers.AddAsync(driver, cancellationToken);
        
        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException("Failed to create driver.");
    }

    public async Task Handle(TestDriverRemovedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test driver {TestDriverKey} removed event has been received.", notification.Key);

        var driver = await db.TestDrivers
            .FindAsync(notification.Key, cancellationToken) ??
            throw new TestsManagerInfrastructureException($"Test driver {notification.Key} not found.");

        db.TestDrivers.Remove(driver);
        
        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException("Failed to delete driver.");
    }

    public async Task Handle(TestDriverValidationStatusEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test driver {TestDriverKey} {AgId} validation result event has been received.",
            notification.Key,
            notification.AgId);

        var driver = await db.TestDrivers
            .FindAsync(notification.Key, cancellationToken) ??
            throw new TestsManagerInfrastructureException($"Test driver {notification.Key} not found.");

        driver.ValidationResult = Enum.Parse<ValidationResult>(notification.ValidationStatus);
        driver.ValidationMessage = notification.ValidationMessage;

        db.TestDrivers.Update(driver);
        
        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException($"Failed to update driver. {notification.Key} {notification.AgId}");
    }
}