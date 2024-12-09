using System.Reflection;

namespace BeaversTests.Common.CQRS.Abstractions;

public abstract class Aggregate
{
    public Guid Id { get; protected set; }
    public int Version { get; protected internal set; } = 0;
    public DateTime CreatedUtc { get; protected internal set; }

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

    internal void Apply<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var eventType = @event.GetType();

        var appliers = GetAggregateAppliers();

        var applier = appliers.FirstOrDefault(a =>
        {
            var parameter = GetApplierEvent(a);
            
            return parameter.ParameterType == eventType;
        }) ?? throw new InvalidOperationException(
            $"Aggregate {GetType()} event applier for {eventType} not found");

        applier.Invoke(this, [ @event ]);
    }

    protected internal Type GetAppliedEventType(string eventTypeData)
    {
        var appliers = GetAggregateAppliers();

        var applier = appliers.FirstOrDefault(a =>
        {
            var parameter = GetApplierEvent(a);
            
            return parameter.ParameterType.GetTypeName() == eventTypeData;
        }) ?? throw new InvalidOperationException("Event type not found.");

        return GetApplierEvent(applier).ParameterType;
    }

    protected virtual void Enqueue(IEvent @event)
    {
        Version++;
        CreatedUtc = DateTime.UtcNow;
        _uncommittedEvents.Add(@event);
    }

    protected internal abstract Aggregate Empty();

    private IEnumerable<MethodInfo> GetAggregateAppliers()
    {
        var aggregateType = GetType();

        var appliers = aggregateType
            .GetMethods()
            .Where(m => m.GetCustomAttribute(EventApplierAttributeType) is not null);

        return appliers;
    }

    private ParameterInfo GetApplierEvent(MethodInfo applier)
    {
        var parameter = applier.GetParameters()
            .SingleOrDefault();

        if (parameter == null ||
            !parameter.ParameterType.IsAssignableTo(EventInterfaceType))
        {
            throw new InvalidOperationException(
                "Event applier method parameter must be one and have the event type");
        }

        return parameter;
    }
}