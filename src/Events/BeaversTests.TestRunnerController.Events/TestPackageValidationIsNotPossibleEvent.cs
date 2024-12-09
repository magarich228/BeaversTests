using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestRunnerController.Events;

public class TestPackageValidationIsNotPossibleEvent : IEvent
{
    /// <summary>
    /// Test package Id.
    /// </summary>
    public required Guid Id { get; init; }
}