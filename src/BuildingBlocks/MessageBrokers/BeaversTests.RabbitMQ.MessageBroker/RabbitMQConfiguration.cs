namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMQConfiguration
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string UserName { get; init; } = null!;
    public string Password { get; init; } = null!;
    public int Retries { get; init; }
    public int RetryTimeoutMilliseconds { get; init; }
    public ExchangeOptions Exchange { get; init; } = null!;
}

public class ExchangeOptions
{
    public string Name { get; init; } = null!;
}