using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerAgent.Events;

public class TestRunnerPreparedEvent : IEvent
{
    public required Guid Id { get; init; }
    public required string ControllerConnectionKey { get; init; }
}