using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerAgent.Events;

namespace BeaversTests.TestRunnerAgent.Core;

public class TestRunnerAggregate : Aggregate
{
    public TestAgentStatus Status { get; private set; } = TestAgentStatus.Created;
    
    [EventApplier]
    public void ApplyPrepared(TestRunnerPreparedEvent @event)
    {
        Id = @event.Id;
        Status = TestAgentStatus.Prepared;
        
        base.Enqueue(@event);
    }
    
    [EventApplier]
    public void ApplyFinalized(TestRunnerFinalizedEvent @event)
    {
        Status = TestAgentStatus.Finalized;
        
        base.Enqueue(@event);
    }
    
    protected override Aggregate Empty()
    {
        return new TestRunnerAggregate();
    }
}