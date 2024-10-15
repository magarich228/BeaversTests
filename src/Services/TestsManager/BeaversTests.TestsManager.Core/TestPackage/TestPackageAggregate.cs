using BeaversTests.Common.CQRS;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.Events.TestPackage;

namespace BeaversTests.TestsManager.Core.TestPackage;

public class TestPackageAggregate : Aggregate
{
    public string TestPackageName { get; private set; } = default!;
    public string? Description { get; private set; }
    public string TestDriverKey { get; private set; } = default!;
    public Guid TestProjectId { get; private set; }
    
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

    protected override Aggregate Empty()
    {
        return new TestPackageAggregate();
    }
}