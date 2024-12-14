using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestDriverValidationTaskEvent : IEvent
{
    public required string Key { get; init; }
    public required Guid AgId { get; init; }
    public required Guid TestAgentId { get; init; }
}