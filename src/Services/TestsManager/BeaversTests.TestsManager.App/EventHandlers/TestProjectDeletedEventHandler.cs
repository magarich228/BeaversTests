using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Events.TestProject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.EventHandlers;

public class TestProjectDeletedEventHandler(
    ITestsManagerContext db,
    ICommandBus commandBus,
    ILogger<TestProjectDeletedEventHandler> logger) : IEventHandler<TestProjectDeletedEvent>
{
    public async Task Handle(TestProjectDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test project {TestProjectId} deleted event has been received.", notification.Id);

        var project = await db.TestProjects
            .Include(t => t.TestPackages)
            .FirstOrDefaultAsync(p => p.Id == notification.Id, cancellationToken);

        if (project == null)
        {
            throw new TestsManagerException($"Test project {notification.Id} not found. Test packages does not deleted.");
        }

        if (project.TestPackages != null)
        {
            foreach (var testPackage in project.TestPackages)
            {
                await commandBus.SendAsync(new RemoveTestPackageCommand.Command()
                {
                    Id = testPackage.Id
                }, cancellationToken);
            }
        }
    }
}