using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestsManager.Events.TestDriver;

public class TestDriverRemovedEvent : IEvent
{
    public required Guid AgId { get; init; }
    public required string UserId { get; init; }
    public required string Key { get; init; }
}