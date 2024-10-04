using MediatR;

namespace BeaversTests.Common.CQRS.Events;

public interface IEventHandler<in T> : INotificationHandler<T> where T : IEvent { }