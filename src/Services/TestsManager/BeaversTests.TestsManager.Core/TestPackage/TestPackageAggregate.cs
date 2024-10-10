using BeaversTests.Common.CQRS;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestsManager.Events.TestPackage;

namespace BeaversTests.TestsManager.Core.TestPackage;

public class TestPackageAggregate : Aggregate
{
    internal TestPackageAggregate() : base()
    {
    }

    protected override Aggregate Empty()
    {
        return new TestPackageAggregate();
    }
}

public class TestPackageAddedApplier : IEventApplier<TestPackageAggregate, TestPackageAdded>
{
    public void Apply(TestPackageAggregate aggregate, TestPackageAdded @event)
    {
        
    }
}