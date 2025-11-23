using BeaversTests.Platform;

namespace BeaversTests.Auth;

public class AuthException : BeaversTestsInternalException
{
    public AuthException(string message) : base(message) { }
    public AuthException(string message, Exception innerException) : base(message, innerException) { }
}