namespace BeaversTests.Isolation;

public class IsolationException(
    string? message,
    Exception? innerException = null) : 
    Exception(message, innerException);