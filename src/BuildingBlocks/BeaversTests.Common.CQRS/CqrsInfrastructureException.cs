namespace BeaversTests.Common.CQRS;

public class CqrsInfrastructureException(
    string? message = null,
    Exception? innerException = null) : Exception(message, innerException) { }