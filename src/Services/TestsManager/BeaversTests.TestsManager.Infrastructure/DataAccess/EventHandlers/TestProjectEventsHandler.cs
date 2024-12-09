using AutoMapper;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Core.TestProject;
using BeaversTests.TestsManager.Events.TestProject;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.DataAccess.EventHandlers;

public class TestProjectEventsHandler(
    ITestsManagerContext db,
    IMapper mapper,
    ILogger<TestProjectEventsHandler> logger) :
    IEventHandler<TestProjectAddedEvent>,
    IEventHandler<TestProjectUpdatedEvent>,
    IEventHandler<TestProjectDeletedEvent>
{
    public async Task Handle(TestProjectAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test project {TestProjectId} added event has been received.", notification.Id);

        var project = mapper.Map<TestProjectAddedEvent, TestProject>(notification);

        await db.TestProjects.AddAsync(project, cancellationToken);

        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException("Failed to create project.");
    }

    public async Task Handle(TestProjectUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test project {TestProjectId} updated event has been received.", notification.Id);

        var project = await db.TestProjects.FindAsync(notification.Id);

        if (project is null)
            throw new TestsManagerInfrastructureException($"Test project {notification.Id} not found.");
        
        project.Name = notification.Name;
        project.Description = notification.Description;

        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException("Failed to update project.");
    }

    public async Task Handle(TestProjectDeletedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test project {TestProjectId} deleted event has been received.", notification.Id);

        var project = await db.TestProjects.FindAsync(
            notification.Id, cancellationToken) ?? 
                      throw new TestsManagerInfrastructureException($"Test project {notification.Id} not found.");
        
        db.TestProjects.Remove(project);
        
        if (await db.SaveChangesAsync(cancellationToken) == 0)
            throw new TestsManagerInfrastructureException("Failed to delete project.");
    }
}