using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestsManager.Events.TestDriver;

public class TestDriverAddedEvent : IEvent
{
    public required Guid Id { get; init; }
    public required string UserId { get; init; }
    public required string Key { get; init; }
    public string? Description { get; init; }
}