using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerAgent.Events;

namespace BeaversTests.TestRunnerController.Core;

public class TestAgentAggregate : Aggregate
{
    public TestAgentStatus Status { get; set; } = TestAgentStatus.Created;

    [EventApplier]
    public void ApplyPrepared(TestRunnerPreparedEvent @event)
    {
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
        return new TestAgentAggregate();
    }
}