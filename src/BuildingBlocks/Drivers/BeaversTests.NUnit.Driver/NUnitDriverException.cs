namespace BeaversTests.NUnit.Driver;

public class NUnitDriverException : Exception
{
    public NUnitDriverException(
        string? message,
        Exception? innerException = null) : 
        base(message, innerException) { }
}