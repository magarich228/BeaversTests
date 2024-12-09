namespace BeaversTests.TestsManager.Infrastructure;

public class TestsManagerInfrastructureException(
    string? message = null,
    Exception? innerException = null) : Exception(message, innerException) { }