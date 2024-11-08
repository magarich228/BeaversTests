using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestRunnerFinalizedEvent : IEvent
{
    public Guid Id { get; init; }
}