using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestsManager.Events.TestPackage;

namespace BeaversTests.TestsManager.Core.TestPackage;

public class TestPackageAggregate : Aggregate
{
    public string TestPackageName { get; private set; } = default!;
    public string? Description { get; private set; }
    public string TestDriverKey { get; private set; } = default!;
    public Guid TestProjectId { get; private set; }
    public ValidationResult TestPackageValidationStatus { get; private set; } = ValidationResult.InProgress;
    public string ValidationMessage { get; private set; } = string.Empty;
    
    public TestPackageAggregate() { }

    [EventApplier]
    public void ApplyCreated(TestPackageAddedEvent @event)
    {
        Id = @event.Id;
        TestPackageName = @event.Name;
        Description = @event.Description;
        TestDriverKey = @event.TestDriverKey;
        TestProjectId = @event.TestProjectId;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyValidationResult(TestPackageValidationStatusEvent @event)
    {
        if (Id != @event.Id)
        {
            throw new ArgumentException("Test package id mismatch.", nameof(@event.Id));
        }
        
        TestPackageValidationStatus = Enum.Parse<ValidationResult>(@event.ValidationStatus);
        ValidationMessage = @event.ValidationMessage;
        
        base.Enqueue(@event);
    }

    protected override Aggregate Empty()
    {
        return new TestPackageAggregate();
    }
}