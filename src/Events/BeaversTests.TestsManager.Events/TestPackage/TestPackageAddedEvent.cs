using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.TestsManager.Events.TestPackage;

public class TestPackageAddedEvent : IEvent
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string TestDriverKey { get; init; }
    public required Guid TestProjectId { get; init; }
}