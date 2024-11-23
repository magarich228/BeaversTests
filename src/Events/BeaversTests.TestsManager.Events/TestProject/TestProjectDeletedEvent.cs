using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestsManager.Events.TestProject;

public class TestProjectDeletedEvent : IEvent
{
    public required Guid Id { get; init; }
}