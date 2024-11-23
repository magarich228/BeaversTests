using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestsManager.Events.TestProject;

namespace BeaversTests.TestsManager.Core.TestProject;

public class TestProjectAggregate : Aggregate
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    
    public TestProjectAggregate() { }
    
    [EventApplier]
    public void ApplyCreated(TestProjectAddedEvent @event)
    {
        Id = @event.Id;
        Name = @event.Name;
        Description = @event.Description;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyUpdated(TestProjectUpdatedEvent @event)
    {
        CheckId(@event.Id);
        
        Name = @event.Name;
        Description = @event.Description;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyDeleted(TestProjectDeletedEvent @event)
    {
        CheckId(@event.Id);
        
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