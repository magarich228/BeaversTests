namespace BeaversTests.TestRunner;

public class TestRunnerException(
    string? message, 
    Exception? innerException = null) : 
    ApplicationException(message, innerException) { }