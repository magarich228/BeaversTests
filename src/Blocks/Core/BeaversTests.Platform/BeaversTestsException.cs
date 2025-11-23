namespace BeaversTests.Platform;

public class BeaversTestsException : Exception
{
    public BeaversTestsException(string message) : base(message) { }
    public BeaversTestsException(string message, Exception innerException) : 
        base(message, innerException) { }
}

// TODO: base class for all platform exceptions