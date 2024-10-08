using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; protected internal set; } = 0;
    public DateTime CreatedUtc { get; protected internal set; }
    public virtual string Name => "";

    [NonSerialized]
    private readonly List<IEvent> _uncommittedEvents = new List<IEvent>();

    protected internal Aggregate()
    { }

    internal IEnumerable<IEvent> DequeueUncommittedEvents()
    {
        var dequeuedEvents = _uncommittedEvents.ToList();
        _uncommittedEvents.Clear();

        return dequeuedEvents;
    }

    public abstract void Apply<TEvent>(TEvent @event) where TEvent : IEvent;

    protected virtual void Enqueue(IEvent @event)
    {
        Version++;
        CreatedUtc = DateTime.UtcNow;
        _uncommittedEvents.Add(@event);
    }

    protected internal abstract Aggregate Empty();
}