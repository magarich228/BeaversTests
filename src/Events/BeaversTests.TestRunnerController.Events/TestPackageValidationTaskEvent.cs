using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestPackageValidationTaskEvent : IEvent
{
    /// <summary>
    /// Test package id.
    /// </summary>
    public required Guid Id { get; init; }
    public required string TestDriverKey { get; init; }
    public required Guid TestAgentId { get; init; }
}