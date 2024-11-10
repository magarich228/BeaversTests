using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerAgent.Events;

public class TestRunnerFinalizedEvent : IEvent
{
    public required Guid Id { get; init; }
}