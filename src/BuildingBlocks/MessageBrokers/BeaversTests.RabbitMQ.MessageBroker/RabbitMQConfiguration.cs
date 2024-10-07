﻿namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMQConfiguration
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    
    public required QueueOptions Queue { get; init; }

    public required ExchangeOptions Exchange { get; init; }
}

public class QueueOptions
{
    public required string Name { get; set; }
}

public class ExchangeOptions
{
    public required string Name { get; set; }
}