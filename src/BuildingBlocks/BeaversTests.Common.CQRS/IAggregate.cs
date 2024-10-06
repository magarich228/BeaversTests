using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

public interface IAggregate
{
    Guid Id { get; }
    int Version { get; }
    DateTime CreatedUtc { get; }

    IEnumerable<IEvent> DequeueUncommittedEvents();
}

public abstract class Aggregate : IAggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; } = 0;
    public DateTime CreatedUtc { get; protected set; }
    public virtual string Name => "";

    [NonSerialized]
    private readonly List<IEvent> _uncommittedEvents = new List<IEvent>();

    protected Aggregate()
    { }

    IEnumerable<IEvent> IAggregate.DequeueUncommittedEvents()
    {
        var dequeuedEvents = _uncommittedEvents.ToList();

        _uncommittedEvents.Clear();

        return dequeuedEvents;
    }

    protected virtual void Enqueue(IEvent @event)
    {
        Version++;
        CreatedUtc = DateTime.UtcNow;
        _uncommittedEvents.Add(@event);
    }
}