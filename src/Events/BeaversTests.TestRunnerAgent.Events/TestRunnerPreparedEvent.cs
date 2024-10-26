using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerAgent.Events;

public class TestRunnerPreparedEvent : IEvent
{
    public Guid Id { get; init; }
}