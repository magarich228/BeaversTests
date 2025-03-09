using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestsManager.Events.TestDriver;

namespace BeaversTests.TestsManager.Core.TestDriver;

public class TestDriverAggregate : Aggregate
{
    public string Key { get; private set; } = null!;
    public string UserCreatorId { get; private set; } = null!;
    public ValidationResult ValidationResult { get; private set; } = ValidationResult.Unknown;
    public string? ValidationMessage { get; private set; }
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; }

    [EventApplier]
    public void ApplyAdded(TestDriverAddedEvent @event)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Test driver deleted.");
        
        if (Version != 0)
            throw new InvalidOperationException("Test driver already created.");
        
        Id = @event.AgId;
        Key = @event.Key;
        UserCreatorId = @event.UserId;
        Description = @event.Description;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyValidationStatus(TestDriverValidationStatusEvent @event)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Test driver already deleted.");
        
        if (Id != @event.AgId)
            throw new InvalidOperationException("Test driver not found.");

        if (Key != @event.Key)
            throw new InvalidOperationException("Test driver key is invalid.");
        
        ValidationResult = Enum.Parse<ValidationResult>(@event.ValidationStatus);
        ValidationMessage = @event.ValidationMessage;
        
        base.Enqueue(@event);
    }
    
    [EventApplier]
    public void ApplyDeleted(TestDriverRemovedEvent @event)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Test driver already deleted.");
        
        if (Id != @event.AgId)
            throw new InvalidOperationException("Test driver not found.");

        if (Key != @event.Key)
            throw new InvalidOperationException("Test driver key is invalid.");
        
        if (UserCreatorId != @event.UserId)
            throw new InvalidOperationException("This user can't delete this test driver.");
        
        Id = @event.AgId;

        IsDeleted = true;
        
        base.Enqueue(@event);
    }
    
    protected override Aggregate Empty()
    {
        return new TestDriverAggregate();
    }
}