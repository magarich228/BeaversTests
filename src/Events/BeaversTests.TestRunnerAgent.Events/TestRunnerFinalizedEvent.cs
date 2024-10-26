using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerAgent.Events;

public class TestRunnerFinalizedEvent : IEvent
{
    public Guid Id { get; init; }
}