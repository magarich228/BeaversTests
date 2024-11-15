using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerController.Events;

namespace BeaversTests.TestRunnerController.Core;

public class TestPackageTasksAggregate : Aggregate
{
    public Guid TestAgentId { get; set; }
    public string DriverKey { get; set; } = string.Empty;
    
    public TestPackageTasksAggregate() { }

    [EventApplier]
    public void ApplyValidationIsNotPossible(TestPackageValidationIsNotPossibleEvent @event)
    {
        Id = @event.Id;
        
        base.Enqueue(@event);
    }

    [EventApplier]
    public void ApplyValidationTask(TestPackageValidationTaskEvent @event)
    {
        Id = @event.Id;
        TestAgentId = @event.TestAgentId;
        DriverKey = @event.TestDriverKey;
        
        base.Enqueue(@event);
    }
    
    protected override Aggregate Empty()
    {
        return new TestPackageTasksAggregate();
    }
}