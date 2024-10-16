using System.Reflection;

namespace BeaversTests.Common.CQRS.Abstractions;

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; protected internal set; } = 0;
    public DateTime CreatedUtc { get; protected internal set; }
    public virtual string Name => "";

    [NonSerialized]
    private readonly List<IEvent> _uncommittedEvents = new();

    private Type EventApplierAttributeType => typeof(EventApplierAttribute);
    private Type EventInterfaceType => typeof(IEvent);

    internal IEnumerable<IEvent> DequeueUncommittedEvents()
    {
        var dequeuedEvents = _uncommittedEvents.ToList();
        _uncommittedEvents.Clear();

        return dequeuedEvents;
    }

    protected internal void Apply<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var aggregateType = GetType();
        var eventType = @event.GetType();
        
        var appliers = aggregateType
            .GetMethods(BindingFlags.Instance)
            .Where(m => m.GetCustomAttribute(EventApplierAttributeType) is not null);

        var applier = appliers.FirstOrDefault(a =>
        {
            var parameter = a.GetParameters()
                .SingleOrDefault();

            if (parameter == null ||
                !parameter.ParameterType.IsAssignableTo(EventInterfaceType))
            {
                throw new InvalidOperationException(
                    "Event applier method parameter must be one and have the event type");
            }
            
            return parameter.ParameterType == eventType;
        }) ?? throw new InvalidOperationException(
            $"Aggregate {aggregateType} event applier for {eventType} not found");

        applier.Invoke(this, [ @event ]);
    }

    protected virtual void Enqueue(IEvent @event)
    {
        Version++;
        CreatedUtc = DateTime.UtcNow;
        _uncommittedEvents.Add(@event);
    }

    protected internal abstract Aggregate Empty();
}