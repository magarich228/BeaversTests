using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerController.Events;

namespace BeaversTests.TestRunnerController.Core;

public class TestAgentAggregate : Aggregate
{
    public TestAgentStatus Status { get; set; } = TestAgentStatus.Created;
    public string OwnerId { get; set; } = null!;

    [EventApplier]
    public void ApplyPrepared(TestRunnerPreparedEvent @event)
    {
        Id = @event.Id;
        OwnerId = @event.OwnerId;
        Status = TestAgentStatus.Prepared;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyFinalized(TestRunnerFinalizedEvent @event)
    {
        Id = @event.Id;
        Status = TestAgentStatus.Finalized;
        
        base.Enqueue(@event);
    }
    
    protected override Aggregate Empty()
    {
        return new TestAgentAggregate();
    }
}