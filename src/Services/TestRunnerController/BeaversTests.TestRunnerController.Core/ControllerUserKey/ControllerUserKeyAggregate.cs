using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerController.Events;

namespace BeaversTests.TestRunnerController.Core;

public class ControllerUserKeyAggregate : Aggregate
{
    public string Key { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    
    [EventApplier]
    public void ApplyCreated(ControllerUserKeyCreatedEvent @event)
    {
        if (Version != 0)
            throw new InvalidOperationException("Controller user key already created");
        
        Id = @event.Id;
        Key = @event.Key;
        OwnerId = @event.OwnerId;
        
        base.Enqueue(@event);
    }
    
    protected override Aggregate Empty()
    {
        return new ControllerUserKeyAggregate();
    }
}