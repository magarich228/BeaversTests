using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerController.Events;

namespace BeaversTests.TestRunnerController.Core;

// TODO: write aggregate tasks to event store, read db.
public class TestDriverTasksAggregate : Aggregate
{
    public string Key { get; private set; } = null!;
    public Guid TestAgentId { get; private set; }

    public void Create(Guid agId, string key)
    {
        Id = agId;
        Key = key;
    }
    
    [EventApplier]
    public void ApplyValidationIsNotPossible(TestDriverValidationIsNotPossibleEvent @event)
    {
        if (Id != @event.AgId || Key != @event.Key)
            throw new InvalidOperationException($"Invalid test driver. Aggregate driver: {Key} {Id}; " +
                                                $"Event driver: {@event.Key} {@event.AgId}");
        
        TestAgentId = Guid.Empty;
        
        base.Enqueue(@event);
    }
    
    protected override Aggregate Empty()
    {
        return new TestDriverTasksAggregate();
    }
}