namespace BeaversTests.RabbitMQ.MessageBroker;

public class RabbitMqException(
    string? message = null,
    Exception? innerException = null) : Exception(message, innerException) {}