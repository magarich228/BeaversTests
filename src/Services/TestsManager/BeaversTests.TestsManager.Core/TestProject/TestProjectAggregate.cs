using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestsManager.Events.TestProject;

namespace BeaversTests.TestsManager.Core.TestProject;

public class TestProjectAggregate : Aggregate
{
    public string UserCreatorId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsDeleted { get; private set; } = false;
    
    [EventApplier]
    public void ApplyCreated(TestProjectAddedEvent @event)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Test project deleted.");
        
        if (Version != 0)
            throw new InvalidOperationException("Test project already created.");
        
        Id = @event.Id;
        UserCreatorId = @event.UserId;
        Name = @event.Name;
        Description = @event.Description;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyUpdated(TestProjectUpdatedEvent @event)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Test project deleted.");
        
        CheckId(@event.Id);

        if (@event.UserId != UserCreatorId)
            throw new Exception("This user can't update test project.");
        
        Name = @event.Name;
        Description = @event.Description;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyDeleted(TestProjectDeletedEvent @event)
    {
        CheckId(@event.Id);

        if (IsDeleted)
            throw new InvalidOperationException("Test project already deleted.");
        
        IsDeleted = true;
        
        if (@event.UserId != UserCreatorId)
            throw new Exception("This user can't delete test project.");
        
        base.Enqueue(@event);
    }
    
    private void CheckId(Guid id)
    {
        if (Id != id)
        {
            throw new ArgumentException("Test project id mismatch.", nameof(id));
        }
    }
    
    protected override Aggregate Empty()
    {
        return new TestProjectAggregate();
    }
}