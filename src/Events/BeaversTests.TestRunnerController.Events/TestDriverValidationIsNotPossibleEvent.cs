using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestDriverValidationIsNotPossibleEvent : IEvent
{
    public required string Key { get; init; }
    public required Guid AgId { get; init; }
}