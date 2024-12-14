using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class ControllerUserKeyCreatedEvent : IEvent
{
    public required Guid Id { get; init; }
    public required string Key { get; init; }
    public required string OwnerId { get; init; }
}