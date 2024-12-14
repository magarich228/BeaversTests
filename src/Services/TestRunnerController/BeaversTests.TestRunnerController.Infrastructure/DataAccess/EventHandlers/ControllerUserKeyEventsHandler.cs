using AutoMapper;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.Infrastructure.DataAccess.EventHandlers;

public class ControllerUserKeyEventsHandler(
    ITestRunnerControllerContext db,
    IMapper mapper,
    ILogger<ControllerUserKeyEventsHandler> logger) : IEventHandler<ControllerUserKeyCreatedEvent>
{
    public async Task Handle(ControllerUserKeyCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Created user controller key for user {UserId}", notification.OwnerId);

        var key = mapper.Map<ControllerUserKey>(notification);

        await db.ControllerUserKeys.AddAsync(key, cancellationToken);
        
        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new ApplicationException($"Failed to save user ({key.OwnerId}) controller key");
    }
}