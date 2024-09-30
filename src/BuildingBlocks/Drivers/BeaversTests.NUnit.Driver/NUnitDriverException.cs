namespace BeaversTests.NUnit.Driver;

public class NUnitDriverException(
    string? message,
    Exception? innerException = null) : Exception(message, innerException);