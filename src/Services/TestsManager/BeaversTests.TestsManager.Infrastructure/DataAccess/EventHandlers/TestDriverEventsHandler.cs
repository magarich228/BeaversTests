using AutoMapper;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.EventHandlers;

public class TestDriverEventsHandler(
    TestsManagerContext db,
    IMapper mapper,
    ILogger<TestDriverEventsHandler> logger) : IEventHandler<TestDriverAddedEvent>
{
    public async Task Handle(TestDriverAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test driver {TestDriverKey} added event has been received.", notification.Key);

        var driver = mapper.Map<TestDriverAddedEvent, TestDriver>(notification);

        await db.TestDrivers.AddAsync(driver, cancellationToken);
        
        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException("Failed to create driver.");
    }
}