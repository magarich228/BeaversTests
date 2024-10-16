using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BeaversTests.RabbitMQ.MessageBroker;

public class Subscription(EventingBasicConsumer consumer, IConnection connection, IModel channel) : IDisposable
{
    public EventingBasicConsumer Consumer { get; } = consumer;
    public IConnection Connection { get; } = connection;
    public IModel Channel { get; } = channel;

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            Connection.Dispose();
            Channel.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~Subscription()
    {
        Dispose(false);
    }
}