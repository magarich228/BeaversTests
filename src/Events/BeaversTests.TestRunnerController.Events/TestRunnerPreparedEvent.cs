using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestRunnerPreparedEvent : IEvent
{
    public Guid Id { get; init; }
}