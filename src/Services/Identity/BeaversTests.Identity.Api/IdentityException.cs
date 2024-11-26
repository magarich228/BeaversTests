namespace BeaversTests.Identity.Api;

public class IdentityException(string message, Exception? innerException = null)
    : Exception(message, innerException) { }