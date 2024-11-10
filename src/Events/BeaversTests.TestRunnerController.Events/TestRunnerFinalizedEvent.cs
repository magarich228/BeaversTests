using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestRunnerFinalizedEvent : IEvent
{
    public required Guid Id { get; init; }
}