using System.Text;
using BeaversTests.Common.CQRS.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMqService : IMessageBroker
{
    private readonly IConnectionFactory _factory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitMqService> _logger;

    private readonly List<Subscription> _subscriptions;

    public RabbitMqService(
        RabbitMQConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitMqService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        var factory = new ConnectionFactory
        {
            Endpoint = new AmqpTcpEndpoint(configuration.Host, configuration.Port),
            UserName = configuration.UserName,
            Password = configuration.Password,
        };

        _factory = factory;
        _logger = logger;

        _subscriptions = new();
    }

    public Task SubscribeAsync(Type type, CancellationToken cancellationToken = default)
    {
        // TODO: type validation

        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();

        var exchangeName = GetExchangeName(type);

        channel.ExchangeDeclare(
            exchange: exchangeName,
            type: ExchangeType.Fanout);

        var queueDeclareOk = channel.QueueDeclare();

        _logger.LogInformation($"Exchange {exchangeName} with queue {queueDeclareOk.QueueName} declared.");

        channel.QueueBind(
            queue: queueDeclareOk.QueueName,
            exchange:exchangeName,
            routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(channel);

        // TODO: move to method
        consumer.Received += async (sender, @event) =>
        {
            _logger.LogInformation(
                $"Received event: RK: {@event.RoutingKey} Ex: {@event.Exchange} Ct: {@event.ConsumerTag}");

            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            // TODO: add custom exception, logging
            var deserializedEvent = JsonConvert.DeserializeObject(message, type) as IEvent
                                    ?? throw new ApplicationException($"Can't deserialize event: {message}");

            await eventBus.PullAsync(cancellationToken, deserializedEvent);
        };

        _logger.LogDebug($"Subscribed to exchange: {exchangeName} with queue: {queueDeclareOk.QueueName}");
        
        channel.BasicConsume(
            queue: queueDeclareOk.QueueName,
            autoAck: true,
            consumerTag: string.Empty,
            consumer: consumer);

        _subscriptions.Add(new Subscription(consumer, connection, channel));

        return Task.CompletedTask;
    }

    public async Task SubscribeAsync<TEvent>(CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        await SubscribeAsync(typeof(TEvent), cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        var message = JsonConvert.SerializeObject(@event);
        var type = GetExchangeName(typeof(TEvent));

        await PublishAsync(message, type, cancellationToken);
    }

    public Task PublishAsync(string message, string type, CancellationToken cancellationToken = default)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            type,
            ExchangeType.Fanout);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: type,
            routingKey: string.Empty,
            basicProperties: null,
            body: body);

        _logger.LogDebug($"Published {type}");

        return Task.CompletedTask;
    }

    // TODO: Перенести в общую сборку.
    private string GetExchangeName(Type type)
    {
        return $"{type.Name}"
            .Replace('+', '.')
            .ToLowerInvariant();
    }

    ~RabbitMqService()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription.Dispose();
        }
    }
}