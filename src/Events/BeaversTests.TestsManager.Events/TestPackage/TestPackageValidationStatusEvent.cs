using BeaversTests.Common.CQRS.Abstractions;

namespace BeaversTests.TestsManager.Events.TestPackage;

public class TestPackageValidationStatusEvent : IEvent
{
    /// <summary>
    /// Test package id.
    /// </summary>
    public required Guid Id { get; init; }
    public required string ValidationStatus { get; init; }
    public required string ValidationMessage { get; init; }
}