using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestPackageValidationTaskEvent : IEvent
{
    public required Guid Id { get; init; }
    public required Guid TestPackageId { get; init; }
    public required string TestDriverKey { get; init; }
    public required Guid TestAgentId { get; init; }
}