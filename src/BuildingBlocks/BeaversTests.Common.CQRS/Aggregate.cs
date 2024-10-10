using BeaversTests.Common.CQRS.Events;

namespace BeaversTests.Common.CQRS;

public interface IEventApplier<in TAggregate, in TEvent>
    where TAggregate : Aggregate
    where TEvent : IEvent
{
    public abstract void Apply(TAggregate aggregate, TEvent @event);
    
    internal Type GetEventType() => typeof(TEvent);
}

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; protected internal set; } = 0;
    public DateTime CreatedUtc { get; protected internal set; }
    public virtual string Name => "";

    private readonly IEnumerable<IEventApplier<Aggregate, IEvent>> _appliers;
    [NonSerialized]
    private readonly List<IEvent> _uncommittedEvents = new();

    protected internal Aggregate(params IEventApplier<Aggregate, IEvent>[] appliers)
    {
        _appliers = appliers;
    }

    internal IEnumerable<IEvent> DequeueUncommittedEvents()
    {
        var dequeuedEvents = _uncommittedEvents.ToList();
        _uncommittedEvents.Clear();

        return dequeuedEvents;
    }

    protected internal void Apply<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventType = typeof(TEvent);

        var eventApplier = _appliers.FirstOrDefault(a => a.GetEventType() == eventType)
            ?? throw new ArgumentException($"{eventType} event cannot applied to aggregate {GetType()}");
        
        eventApplier.Apply(this, @event);
    }

    protected virtual void Enqueue(IEvent @event)
    {
        Version++;
        CreatedUtc = DateTime.UtcNow;
        _uncommittedEvents.Add(@event);
    }

    protected internal abstract Aggregate Empty();
}