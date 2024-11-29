using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestsManager.Events.TestProject;

public class TestProjectUpdatedEvent : IEvent
{
    public required Guid Id { get; init; }
    public required string UserId { get; init; } 
    public required string Name { get; init; }
    public string? Description { get; init; }
}