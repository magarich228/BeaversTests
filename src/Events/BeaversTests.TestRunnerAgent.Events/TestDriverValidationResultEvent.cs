using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerAgent.Events;

public class TestDriverValidationResultEvent : IEvent
{
    public required string Key { get; init; }
    public required Guid AgId { get; init; }
    public required string ValidationStatus { get; init; }
    public required string ValidationMessage { get; init; }
}