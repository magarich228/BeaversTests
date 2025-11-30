using BeaversTests.Platform.Public;

namespace BeaversTests.Platform;

public class BeaversTestsInternalException : BeaversTestsException
{
    public BeaversTestsInternalException(string message) : base(message) { }
    public BeaversTestsInternalException(string message, Exception innerException) : 
        base(message, innerException) { }
}